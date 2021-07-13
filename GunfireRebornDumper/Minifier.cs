using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;

namespace GunfireRebornDumper
{
    public static class Minifier
    {
        static List<String> neededTypes = new List<String>();
        static List<String> neededMethods = new List<String>();
        static List<String> neededFields = new List<String>();
        static List<String> neededProperties = new List<String>();
        static List<String> blacklistTypes = new List<String> { "Enum", "ValueType" };
        static List<String> blacklistedAssemblies = new List<String> { "mscorlib", "System", "System.Core", "UnhollowerBaseLib", "UnhollowerRuntimeLib", "Iced", "Mono.Cecil", "Costura" };
        //static List<String> blacklistedAssemblies = new List<String> { "System", "System.Core", "UnhollowerBaseLib", "UnhollowerRuntimeLib", "Iced", "Mono.Cecil", "Costura" };
        static void AddNeededType(TypeReference type)
        {
            if (type.ContainsGenericParameter) return;
            if (blacklistedAssemblies.Contains(type.Scope.Name)) return;
            if (type.FullName.Contains("<") && type.FullName.StartsWith("Il2"))
            {
                if (!neededTypes.Contains(type.FullName.Substring(0, type.FullName.IndexOf("<")))) neededTypes.Add((type.FullName.Substring(0, type.FullName.IndexOf("<"))));

            }
            if (!neededTypes.Contains(type.FullName)) neededTypes.Add(type.FullName);
            if (blacklistTypes.Contains(type.Name)) return;
            var res = type.Resolve();
            var baseType = type.Resolve().BaseType;
            if (baseType != null) AddNeededType(baseType);
        }
        static List<String> Arrays = new List<String> { "Il2CppArrayBase`1", "Il2CppStructArray`1", "Il2CppReferenceArray`1" };
        static String GetCorrectMethodName(MethodDefinition method)
        {
            var returnType = method.ReturnType.FullName;
            if (Arrays.Contains(method.ReturnType.Name))
            {
                returnType = ((GenericInstanceType)method.ReturnType).GenericArguments[0].FullName + "[]";
            }
            var argTypes = new List<String>();
            foreach (var arg in method.Parameters)
            {
                var argType = arg.ParameterType.FullName;
                if (Arrays.Contains(arg.ParameterType.Name))
                {
                    argType = ((GenericInstanceType)arg.ParameterType).GenericArguments[0].FullName + "[]";
                }
                argTypes.Add(argType);
            }
            var argString = String.Join(",", argTypes);
            return returnType + " " + method.DeclaringType.FullName + "::" + method.Name + "(" + argString + ")";
        }
        public static Boolean CleanType(TypeDefinition type)
        {
            //type.BaseType = null;
            //type.Interfaces.Clear();
            var deleteNestedTypes = new List<TypeDefinition>();
            foreach (var nestedType in type.NestedTypes)
            {
                var deleteNestedType = CleanType(nestedType);
                if (deleteNestedType) deleteNestedTypes.Add(nestedType);
            }
            if (type.FullName.Contains("ServerDefine"))
                Console.WriteLine("");
            deleteNestedTypes.ForEach(t => type.NestedTypes.Remove(t));
            if (!neededTypes.Contains(type.FullName))
            {
                return type.NestedTypes.Count == 0;
            }
            if (type.IsEnum) return false;
            var deleteMethods = new List<MethodDefinition>();
            foreach (var method in type.Methods)
            {
                var gen = false;
                if (!(neededMethods.Contains(method.FullName)))
                {
                    deleteMethods.Add(method);
                }
            }
            deleteMethods.ForEach(t => type.Methods.Remove(t.Resolve()));
            var deleteProperties = new List<PropertyDefinition>();
            foreach (var field in type.Properties)
            {
                if (!(neededProperties.Contains(field.FullName)))
                {
                    deleteProperties.Add(field);
                }
            }
            deleteProperties.ForEach(t => type.Properties.Remove(t.Resolve()));
            var deleteFields = new List<FieldDefinition>();
            foreach (var field in type.Fields)
            {
                if (!(neededFields.Contains(field.FullName) || type.IsValueType))
                {
                    deleteFields.Add(field);
                }
            }
            deleteFields.ForEach(t => type.Fields.Remove(t.Resolve()));
            return false;
        }
        static List<IMetadataScope> neededDlls = new List<IMetadataScope>();
        public static void Minify(String exe)
        {
            var assembly = AssemblyDefinition.ReadAssembly(exe);
            var dlls = Directory.GetFiles("DummyDll", "*.dll");

            foreach (EmbeddedResource resource in assembly.MainModule.Resources)
            {
                if (!resource.Name.EndsWith("dll.compressed")) continue;
                //if (blacklistedAssemblies.Count(a => "costura." + a.ToLower() + ".dll.compressed" == resource.Name) > 0) continue;
                if (resource.Name.Contains("mono.cecil")) continue;
                using (var compressedStream = resource.GetResourceStream())
                {
                    using (var deflateStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
                    {
                        using (var outputStream = new MemoryStream())
                        {
                            deflateStream.CopyTo(outputStream);
                            outputStream.Position = 0;
                            var embeddedAssembly = AssemblyDefinition.ReadAssembly(outputStream);
                            File.WriteAllBytes(embeddedAssembly.MainModule.Name, outputStream.ToArray());
                        }
                    }
                }
            }

            foreach (var module in assembly.Modules)
            {
                foreach (var type in module.Types)
                {
                    foreach (var m in type.Methods)
                    {
                        if (!m.HasBody) continue;
                        var methodReferences = m.Body.Instructions.ToList().FindAll(il => /*il.OpCode == Mono.Cecil.Cil.OpCodes.Call &&*/ il.Operand as MethodReference != null).Select(il => il.Operand as MethodReference);
                        foreach (var reference in methodReferences)
                        {
                            if (!blacklistedAssemblies.Contains(reference.DeclaringType.Scope.Name) && reference.DeclaringType.Scope != type.Scope)
                            {
                                var property = reference.DeclaringType.Resolve().Properties.FirstOrDefault(p => p.SetMethod == reference.Resolve() || p.GetMethod == reference.Resolve());
                                if (property != null)
                                {
                                    if (!neededProperties.Contains(property.FullName.Replace("Il2Cpp", "")))
                                    {
                                        neededProperties.Add(property.FullName.Replace("Il2Cpp", ""));
                                    }
                                    if (!neededFields.Contains(property.FullName.Replace("Il2Cpp", "").Replace("()", "")))
                                    {
                                        neededFields.Add(property.FullName.Replace("Il2Cpp", "").Replace("()", ""));
                                    }
                                }
                                if (!neededDlls.Contains(reference.DeclaringType.Scope))
                                {
                                    neededDlls.Add(reference.DeclaringType.Scope);
                                }
                                AddNeededType(reference.DeclaringType);
                                if (reference.IsGenericInstance)
                                {
                                    var genReference = (GenericInstanceMethod)reference;
                                    foreach (var gp in genReference.GenericParameters)
                                    {
                                        AddNeededType(gp);
                                    }
                                }
                                AddNeededType(reference.ReturnType);
                                foreach (var p in reference.Parameters)
                                {
                                    AddNeededType(p.ParameterType);
                                }
                                foreach (var p in reference.DeclaringType.Resolve().Interfaces)
                                {
                                    AddNeededType(p.InterfaceType);
                                }
                                var methodName = reference.FullName;
                                var method = reference.Resolve();
                                methodName = GetCorrectMethodName(method).Replace("Il2Cpp", "");
                                if (!neededMethods.Contains(methodName))
                                {
                                    neededMethods.Add(methodName);
                                }
                                if (reference.IsGenericInstance)
                                {
                                    if (!neededMethods.Contains(reference.Name))
                                    {
                                        //neededMethods.Add(reference.Name);
                                    }
                                }
                            }
                        }
                        var fieldReferences = m.Body.Instructions.ToList().FindAll(il => il.Operand as FieldReference != null).Select(il => il.Operand as FieldReference);
                        foreach (var reference in fieldReferences)
                        {
                            if (!blacklistedAssemblies.Contains(reference.DeclaringType.Scope.Name) && reference.DeclaringType.Scope != type.Scope)
                            {
                                if (!neededDlls.Contains(reference.DeclaringType.Scope))
                                {
                                    neededDlls.Add(reference.DeclaringType.Scope);
                                }
                                AddNeededType(reference.DeclaringType);
                                AddNeededType(reference.FieldType);
                                if (!neededFields.Contains(reference.FullName))
                                {
                                    neededFields.Add(reference.FullName);
                                }
                            }
                        }
                    }
                }
            }
            foreach (var dll in dlls)
            {
                if (neededDlls.Count(d => d.Name == Path.GetFileNameWithoutExtension(dll)) == 0)
                {
                    continue;
                }
                assembly = AssemblyDefinition.ReadAssembly(dll);
                var module = assembly.MainModule;

                var deleteTypes = new List<TypeDefinition>();
                foreach (var type in module.Types)
                {
                    var needRemove = CleanType(type);
                    if (needRemove) deleteTypes.Add(type);
                }
                deleteTypes.ForEach(t => module.Types.Remove(t.Resolve()));

                assembly.Write(dll.Replace("DummyDll", "StrippedDll"));
            }
            Console.WriteLine("");
        }
    }
}
