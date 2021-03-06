﻿// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharpExtensions;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteInvocationExpression
    {
        public static void Go(OutputWriter writer, InvocationExpressionSyntax invocationExpression)
        {
            var symbolInfo = TypeProcessor.GetSymbolInfo(invocationExpression);
            var expressionSymbol = TypeProcessor.GetSymbolInfo(invocationExpression.Expression);

            var symbol = symbolInfo.Symbol ?? symbolInfo.CandidateSymbols.FirstOrDefault(); // Resolution error

            var methodSymbol = symbol.OriginalDefinition.As<IMethodSymbol>().UnReduce();

            var memberReferenceExpressionOpt = invocationExpression.Expression as MemberAccessExpressionSyntax;
            var firstParameter = true;

            var extensionNamespace = methodSymbol.IsExtensionMethod
                ? methodSymbol.ContainingNamespace.FullNameWithDot() + methodSymbol.ContainingType.FullName()
                : null; //null means it's not an extension method, non-null means it is
            string methodName;
            string typeParameters = null;
            ExpressionSyntax subExpressionOpt;

            if (methodSymbol.ContainingType.FullName() == "Enum")
            {
                if (methodSymbol.Name == "Parse")
                {
                    WriteEnumParse(writer, invocationExpression);
                    return;
                }

                if (methodSymbol.Name == "GetValues")
                {
                    WriteEnumGetValues(writer, invocationExpression);
                    return;
                }
            }

            if (expressionSymbol.Symbol is IEventSymbol)
            {
                methodName = "Invoke";
                    //Would need to append the number of arguments to this to support events.  However, events are not currently supported
            }
                //            else if (memberReferenceExpressionOpt != null && memberReferenceExpressionOpt.Expression is PredefinedTypeSyntax)
                //            {
                //                switch (methodSymbol.Name)
                //                {
                //                    case "Parse":
                //                        Core.Write(writer, invocationExpression.ArgumentList.Arguments.Single().Expression);
                //
                //                        writer.Write(".to");
                //                        writer.Write(TypeProcessor.ConvertType(methodSymbol.ReturnType));
                //
                //                        return;
                //                    case "TryParse":
                //                        methodName = "TryParse" + TypeProcessor.ConvertType(methodSymbol.Parameters[1].Type);
                //                        extensionNamespace = "SharpNative";
                //                        break;
                //                    default:
                //                        methodName = methodSymbol.Name;
                //                        extensionNamespace = "SharpNative";
                //                        break;
                //                }
                //            }

            else if (methodSymbol.MethodKind == MethodKind.DelegateInvoke)
                methodName = null;
            else
                methodName = OverloadResolver.MethodName(methodSymbol);

            if (methodSymbol.MethodKind == MethodKind.DelegateInvoke)
                subExpressionOpt = invocationExpression.Expression;
            else if (memberReferenceExpressionOpt != null)
            {
                if (memberReferenceExpressionOpt.Expression is PredefinedTypeSyntax)
                    subExpressionOpt = null;
                else
                    subExpressionOpt = memberReferenceExpressionOpt.Expression;
            }
            else
                subExpressionOpt = null;

            //When the code specifically names generic arguments, include them in the method name ... dlang needs help with inference, so we give it the types anyway
            if (methodSymbol.IsGenericMethod)
            {
                //				var genNameExpression = invocationExpression.Expression as GenericNameSyntax;
                //				if (genNameExpression == null && memberReferenceExpressionOpt != null)
                //					genNameExpression = memberReferenceExpressionOpt.Name as GenericNameSyntax;
                //				if (genNameExpression != null && genNameExpression.TypeArgumentList.Arguments.Count > 0)
                typeParameters = "!( " +
                                 string.Join(", ",
                                     (symbol as IMethodSymbol).TypeArguments.Select(r => TypeProcessor.ConvertType(r))) +
                                 " )";
            }

            //Determine if it's an extension method called in a non-extension way.  In this case, just pretend it's not an extension method
            if (extensionNamespace != null && subExpressionOpt != null &&
                TypeProcessor.GetTypeInfo(subExpressionOpt).ConvertedType.ToString() ==
                methodSymbol.ContainingNamespace + "." + methodSymbol.ContainingType.FullName())
                extensionNamespace = null;

            var memberType = memberReferenceExpressionOpt == null
                ? null
                : TypeProcessor.GetTypeInfo(memberReferenceExpressionOpt.Expression).Type;
            var isNullableEnum = memberType != null &&
                                 (memberType.Name == "Nullable" && memberType.ContainingNamespace.FullName() == "System") &&
                                 memberType.As<INamedTypeSymbol>().TypeArguments.Single().TypeKind == TypeKind.Enum;
            //			if (isNullableEnum && methodSymbol.Name == "ToString")
            //			{
            //				extensionNamespace = null; //override Translations.xml for nullable enums. We want them to convert to the enum's ToString method
            //				methodName = "toString";
            //			}

            var directInvocationOnBasics = methodSymbol.ContainingType.IsBasicType();

            //Extension methods in Dlang are straightforward, although this could lead to clashes without qualification

            if (extensionNamespace != null || directInvocationOnBasics)
            {
                if (extensionNamespace == null)
                {
                    extensionNamespace = memberType.ContainingNamespace.FullName() + "." + memberType.Name;
                        //memberType.ContainingNamespace.FullName() +"."+ memberType.Name;
                }

                writer.Write(extensionNamespace);

                if (methodName != null)
                {
                    writer.Write(".");
                    writer.Write(methodName);
                }

                WriteTypeParameters(writer, typeParameters, invocationExpression);

                writer.Write("(");

                if (subExpressionOpt != null)
                {
                    firstParameter = false;
                    Core.Write(writer, subExpressionOpt);
                }
            }
            else
            {
                if (memberReferenceExpressionOpt != null)
                {
                    //Check against lowercase toString since it gets replaced with the lowered version before we get here
                    if (methodName == "toString")
                    {
                        if (memberType.TypeKind == TypeKind.Enum || isNullableEnum)
                        {
                            var enumType = memberType.TypeKind == TypeKind.Enum
                                ? memberType
                                : memberType.As<INamedTypeSymbol>().TypeArguments.Single();

                            //calling ToString() on an enum forwards to our enum's special ToString method
                            writer.Write(enumType.ContainingNamespace.FullNameWithDot());
                            writer.Write(WriteType.TypeName((INamedTypeSymbol) enumType));
                            writer.Write(".ToString(");
                            Core.Write(writer, memberReferenceExpressionOpt.Expression);
                            writer.Write(")");

                            if (invocationExpression.ArgumentList.Arguments.Count > 0)
                            {
                                throw new Exception(
                                    "Enum's ToString detected with parameters.  These are not supported " +
                                    Utility.Descriptor(invocationExpression));
                            }

                            return;
                        }

                        if (memberType.SpecialType == SpecialType.System_Byte)
                        {
                            //Calling ToString on a byte needs to take special care since it's signed in the JVM
                            writer.Write("System.SharpNative.ByteToString(");
                            Core.Write(writer, memberReferenceExpressionOpt.Expression);
                            writer.Write(")");

                            if (invocationExpression.ArgumentList.Arguments.Count > 0)
                            {
                                throw new Exception(
                                    "Byte's ToString detected with parameters.  These are not supported " +
                                    Utility.Descriptor(invocationExpression));
                            }

                            return;
                        }
                    }
                }

                if (subExpressionOpt != null)
                {
                    WriteMemberAccessExpression.WriteMember(writer, subExpressionOpt);

                    //                    if (!(subExpressionOpt is BaseExpressionSyntax))
                    //                    {
                    //                        if (methodName != null && methodSymbol.IsStatic)
                    //                            writer.Write(".");
                    //                        else
                    //						{	
                    if (methodSymbol.MethodKind != MethodKind.DelegateInvoke)
                        writer.Write(".");
                    //                        }
                    //                    }
                    //					writer.Write(".");
                }
                else if (methodSymbol.IsStatic && extensionNamespace == null)
                {
                    var str = TypeProcessor.ConvertType(methodSymbol.ContainingType);
                    if (str == "Array_T")
                        // Array is the only special case, otherwise generics have to be specialized to access static members
                        str = "Array";

                    writer.Write(str);
                    //                    writer.Write(methodSymbol.ContainingNamespace.FullNameWithDot());
                    //                    writer.Write(WriteType.TypeName(methodSymbol.ContainingType));
                    writer.Write(".");
                }

                if (methodSymbol.MethodKind != MethodKind.DelegateInvoke)
                {
                    var declaringSyntaxReferences =
                        methodSymbol.DeclaringSyntaxReferences.Select(j => j.GetSyntax())
                            .OfType<MethodDeclarationSyntax>();
                    var any = declaringSyntaxReferences.Any();
                    if (any &&
                        declaringSyntaxReferences.FirstOrDefault()
                            .As<MethodDeclarationSyntax>()
                            .Modifiers.Any(SyntaxKind.NewKeyword))
                    {
                        //TODO: this means that new is not supported on external libraries
                        //					//why doesnt roslyn give me this information ?
                        methodName += "_";
                    }

                    if (any &&
                        declaringSyntaxReferences.FirstOrDefault()
                            .As<MethodDeclarationSyntax>()
                            .Modifiers.Any(SyntaxKind.NewKeyword))
                        writer.Write(TypeProcessor.ConvertType(methodSymbol.ContainingType) + ".");

                    //TODO: fix this for abstract too or whatever other combination
                    //TODO: create a better fix fot this
                    //					ISymbol interfaceMethod =
                    //						methodSymbol.ContainingType.AllInterfaces.SelectMany (
                    //							u =>
                    //						u.GetMembers (methodName)
                    //						.Where (
                    //								o =>
                    //							methodSymbol.ContainingType.FindImplementationForInterfaceMember (o) ==
                    //								methodSymbol)).FirstOrDefault ();

                    /*      if (methodSymbol.ContainingType.TypeKind == TypeKind.Interface ||
                Equals(methodSymbol.ContainingType.FindImplementationForInterfaceMember(methodSymbol), methodSymbol))

                              if ((interfaceMethod != null /*&& interfaceMethod == methodSymbol || methodSymbol.ContainingType.TypeKind == TypeKind.Interface)
                          {
                              //This is an interface method //TO

                              var typenameM = Regex.Replace (TypeProcessor.ConvertType (methodSymbol.ContainingType), @" ?!\(.*?\)", string.Empty);
                              if (typenameM.Contains ('.'))
                                  typenameM = typenameM.SubstringAfterLast ('.');
                              if (methodSymbol.ContainingType.SpecialType == SpecialType.System_Array)
                                  writer.Write ("");
                              else if (interfaceMethod != null)
                              {
                                  var typenameI = Regex.Replace (TypeProcessor.ConvertType (interfaceMethod.ContainingType), @" ?!\(.*?\)", string.Empty);
                                  if (typenameI.Contains ('.'))
                                      typenameI = typenameI.SubstringAfterLast ('.');						
                                  writer.Write (typenameI + "_");
                              }
                              else // this is the interface itself
                                      writer.Write (typenameM + "_");

                          }*/

                    if (methodSymbol.ContainingType.TypeKind == TypeKind.Interface ||
                        Equals(methodSymbol.ContainingType.FindImplementationForInterfaceMember(methodSymbol),
                            methodSymbol))
                    {
                        methodName =
                            Regex.Replace(
                                TypeProcessor.ConvertType(methodSymbol.ContainingType.OriginalDefinition)
                                    .RemoveFromStartOfString(methodSymbol.ContainingNamespace + ".Namespace.") + "_" +
                                methodName,
                                @" ?!\(.*?\)", string.Empty);
                    }

                    var interfaceMethods =
                        methodSymbol.ContainingType.AllInterfaces.SelectMany(
                            u =>
                                u.GetMembers(methodName)).ToArray();

                    ISymbol interfaceMethod =
                        interfaceMethods.FirstOrDefault(
                            o => methodSymbol.ContainingType.FindImplementationForInterfaceMember(o) == methodSymbol);

                    //                    if (interfaceMethod == null)
                    //                    {
                    //                        //TODO: fix this for virtual method test 7, seems roslyn cannot deal with virtual 
                    //                        // overrides of interface methods ... so i'll provide a kludge
                    //                        if (!method.Modifiers.Any(SyntaxKind.NewKeyword))
                    //                            interfaceMethod = interfaceMethods.FirstOrDefault(k => CompareMethods(k as IMethodSymbol, methodSymbol));
                    //                    }

                    if (interfaceMethod != null)
                        // && CompareMethods(interfaceMethod ,methodSymbol)) {
                    {
//This is an interface method //TO
                        if (methodSymbol.ContainingType.SpecialType == SpecialType.System_Array)
                            writer.Write("");
                        else
                        {
                            var typenameI =
                                Regex.Replace(
                                    TypeProcessor.ConvertType(interfaceMethod.ContainingType.ConstructedFrom, true),
                                    @" ?!\(.*?\)", string.Empty);
                                //TODO: we should be able to get the original interface name, or just remove all generics from this
                            if (typenameI.Contains('.'))
                                typenameI = typenameI.SubstringAfterLast('.');
                            writer.Write(typenameI + "_");
                        }
                    }

                    //					var acc = methodSymbol.DeclaredAccessibility;

                    //					if (methodSymbol.MethodKind == MethodKind.ExplicitInterfaceImplementation)
                    //					{
                    //						var implementations = methodSymbol.ExplicitInterfaceImplementations[0];
                    //						if (implementations != null)
                    //						{
                    //							//  explicitHeaderNAme = implementations.Name;
                    //							methodName = TypeProcessor.ConvertType(implementations.ReceiverType) + "_" +implementations.Name; //Explicit fix ?
                    //
                    //							//			writer.Write(methodSymbol.ContainingType + "." + methodName);
                    //							//Looks like internal classes are not handled properly here ...
                    //						}
                    //					}

                    writer.Write(methodName);
                }
                WriteTypeParameters(writer, typeParameters, invocationExpression);
                writer.Write("(");
            }

            bool inParams = false;
            bool foundParamsArray = false;

            var arguments = invocationExpression.ArgumentList.Arguments;

            foreach (var arg in arguments.Select(o => new TransformedArgument(o)))
            {
                if (firstParameter)
                    firstParameter = false;
                else
                    writer.Write(", ");

                var argumentType = TypeProcessor.GetTypeInfo(arg.ArgumentOpt.Expression);

                //				if (!inParams && IsParamsArgument (invocationExpression, arg.ArgumentOpt, methodSymbol))
                //				{
                //					foundParamsArray = true;
                //
                //					if (!TypeProcessor.ConvertType (TypeProcessor.GetTypeInfo (arg.ArgumentOpt.Expression).Type).StartsWith ("System.Array<"))
                //					{
                //						inParams = true;
                //						writer.Write ("Array_T!(");
                //					}
                //				}

                //Not needed for dlang
                //                if (arg != null
                //                    && arg.ArgumentOpt.RefOrOutKeyword.RawKind != (decimal) SyntaxKind.None
                //                    && TypeProcessor.GetSymbolInfo(arg.ArgumentOpt.Expression).Symbol is IFieldSymbol)
                //                {
                //                    
                //                }
                //                    throw new Exception("ref/out cannot reference fields, only local variables.  Consider using ref/out on a local variable and then assigning it into the field. " + Utility.Descriptor(invocationExpression));

                //				if (argumentType.Type == null) {
                //					if (argumentType.ConvertedType == null)
                //						writer.Write ("null");
                //					else
                //						writer.Write ("(cast("+TypeProcessor.ConvertType(argumentType.ConvertedType)+") null)");
                //				}
                //               
                //                else if (argumentType.Type.IsValueType && !argumentType.ConvertedType.IsValueType)
                //                {
                //                    //Box
                //					writer.Write("BOX!("+TypeProcessor.ConvertType(argumentType.Type) +")(");
                //                    //When passing an argument by ref or out, leave off the .Value suffix
                //                    if (arg != null && arg.ArgumentOpt.RefOrOutKeyword.RawKind != (decimal)SyntaxKind.None)
                //                        WriteIdentifierName.Go(writer, arg.ArgumentOpt.Expression.As<IdentifierNameSyntax>(), true);
                //                    else
                //                        arg.Write(writer);
                //
                //                    writer.Write(")");
                //                }
                //                else if (!argumentType.Type.IsValueType && argumentType.ConvertedType.IsValueType)
                //                {
                //                    //UnBox
                //					writer.Write("cast(" + TypeProcessor.ConvertType(argumentType.Type) + ")(");
                //                    if (arg != null && arg.ArgumentOpt.RefOrOutKeyword.RawKind != (decimal)SyntaxKind.None)
                //                        WriteIdentifierName.Go(writer, arg.ArgumentOpt.Expression.As<IdentifierNameSyntax>(), true);
                //                    else
                //                        arg.Write(writer);
                //                    writer.Write(")");
                //                }
                //                else
                //                {
                //                    if (arg != null && arg.ArgumentOpt.RefOrOutKeyword.RawKind != (decimal)SyntaxKind.None)
                //                        WriteIdentifierName.Go(writer, arg.ArgumentOpt.Expression.As<IdentifierNameSyntax>(), true);
                //                    else
                //                        arg.Write(writer);
                //                }

                ProcessArgument(writer, arg.ArgumentOpt);
            }

            //			if (inParams)
            //				writer.Write (")");
            //			 if (!foundParamsArray && methodSymbol.Parameters.Any () && methodSymbol.Parameters.Last ().IsParams)
            //				writer.Write (", null"); //params method called without any params argument.  Send null.

            writer.Write(")");
        }


        private static void ProcessArgument(OutputWriter writer, ArgumentSyntax variable)
        {
            if (variable != null)
            {
                if (variable.NameColon != null)
                {
                }

                if (CSharpExtensions.CSharpKind(variable) == SyntaxKind.CollectionInitializerExpression)
                    return;
                var value = variable;
                var initializerType = TypeProcessor.GetTypeInfo(value.Expression);
                var memberaccessexpression = value.Expression as MemberAccessExpressionSyntax;
                var nameexpression = value.Expression as NameSyntax;
                var nullAssignment = value.ToFullString().Trim() == "null";
                var shouldBox = initializerType.Type != null && initializerType.Type.IsValueType &&
                                !initializerType.ConvertedType.IsValueType;
                var shouldUnBox = initializerType.Type != null && !initializerType.Type.IsValueType &&
                                  initializerType.ConvertedType.IsValueType;
                var isname = value.Expression is NameSyntax;
                var ismemberexpression = value.Expression is MemberAccessExpressionSyntax ||
                                         (isname &&
                                          TypeProcessor.GetSymbolInfo(value.Expression as NameSyntax).Symbol.Kind ==
                                          SymbolKind.Method);
                var isdelegateassignment = ismemberexpression &&
                                           initializerType.ConvertedType.TypeKind == TypeKind.Delegate;
                var isstaticdelegate = isdelegateassignment &&
                                       ((memberaccessexpression != null &&
                                         TypeProcessor.GetSymbolInfo(memberaccessexpression).Symbol.IsStatic) ||
                                        (isname && TypeProcessor.GetSymbolInfo(nameexpression).Symbol.IsStatic));

                if (nullAssignment)
                {
                    writer.Write("null");
                    return;
                }

                if (shouldBox)
                {
                    bool useType = true;

                    //We should start with exact converters and then move to more generic convertors i.e. base class or integers which are implicitly convertible
                    var correctConverter = initializerType.Type.GetImplicitCoversionOp(initializerType.ConvertedType,
                        initializerType.Type);
                    //                            initializerType.Type.GetMembers("op_Implicit").OfType<IMethodSymbol>().FirstOrDefault(h => h.ReturnType == initializerType.Type && h.Parameters[0].Type == initializerType.ConvertedType);

                    if (correctConverter == null)
                    {
                        useType = false;
                        correctConverter =
                            initializerType.ConvertedType.GetImplicitCoversionOp(initializerType.ConvertedType,
                                initializerType.Type);
                            //.GetMembers("op_Implicit").OfType<IMethodSymbol>().FirstOrDefault(h => h.ReturnType == initializerType.Type && h.Parameters[0].Type == initializerType.ConvertedType);
                    }

                    if (correctConverter != null)
                    {
                        if (useType)
                        {
                            writer.Write(TypeProcessor.ConvertType(initializerType.Type) + "." + "op_Implicit_" +
                                         TypeProcessor.ConvertType(correctConverter.ReturnType));
                        }
                        else
                        {
                            writer.Write(TypeProcessor.ConvertType(initializerType.ConvertedType) + "." + "op_Implicit_" +
                                         TypeProcessor.ConvertType(correctConverter.ReturnType));
                        }
                        writer.Write("(");
                        Core.Write(writer, value.Expression);
                        writer.Write(")");
                        return;
                    }
                }
                else if (shouldUnBox)
                {
                    bool useType = true;

                    //We should start with exact converters and then move to more generic convertors i.e. base class or integers which are implicitly convertible
                    var correctConverter = initializerType.Type.GetImplicitCoversionOp(initializerType.Type,
                        initializerType.ConvertedType);
                    //                            initializerType.Type.GetMembers("op_Implicit").OfType<IMethodSymbol>().FirstOrDefault(h => h.ReturnType == initializerType.Type && h.Parameters[0].Type == initializerType.ConvertedType);

                    if (correctConverter == null)
                    {
                        useType = false;
                        correctConverter =
                            initializerType.ConvertedType.GetImplicitCoversionOp(initializerType.Type,
                                initializerType.ConvertedType);
                            //.GetMembers("op_Implicit").OfType<IMethodSymbol>().FirstOrDefault(h => h.ReturnType == initializerType.Type && h.Parameters[0].Type == initializerType.ConvertedType);
                    }

                    if (correctConverter != null)
                    {
                        if (useType)
                        {
                            writer.Write(TypeProcessor.ConvertType(initializerType.Type) + "." + "op_Implicit_" +
                                         TypeProcessor.ConvertType(correctConverter.ReturnType));
                        }
                        else
                        {
                            writer.Write(TypeProcessor.ConvertType(initializerType.ConvertedType) + "." + "op_Implicit_" +
                                         TypeProcessor.ConvertType(correctConverter.ReturnType));
                        }
                        writer.Write("(");
                        Core.Write(writer, value.Expression);
                        writer.Write(")");
                        return;
                    }
                }

                if (shouldBox)
                {
                    //Box
                    writer.Write("BOX!(" + TypeProcessor.ConvertType(initializerType.Type) + ")(");
                    //When passing an argument by ref or out, leave off the .Value suffix
                    Core.Write(writer, value.Expression);
                    writer.Write(")");
                    return;
                }
                if (shouldUnBox)
                {
                    //UnBox
                    writer.Write("cast!(" + TypeProcessor.ConvertType(initializerType.Type) + ")(");
                    Core.Write(writer, value.Expression);
                    writer.Write(")");
                }
                if (isdelegateassignment)
                {
                    var typeString = TypeProcessor.ConvertType(initializerType.ConvertedType);

                    var createNew = !(value.Expression is ObjectCreationExpressionSyntax);

                    if (createNew)
                    {
                        if (initializerType.ConvertedType.TypeKind == TypeKind.TypeParameter)
                            writer.Write(" __TypeNew!(" + typeString + ")(");
                        else
                            writer.Write("new " + typeString + "(");
                    }

                    var isStatic = isstaticdelegate;
                    if (isStatic)
                        writer.Write("__ToDelegate(");
                    writer.Write("&");

                    Core.Write(writer, value.Expression);
                    if (isStatic)
                        writer.Write(")");

                    if (createNew)
                        writer.Write(")");
                    return;
                }
                if (initializerType.Type == null && initializerType.ConvertedType == null)
                {
                    writer.Write("null");
                    return;
                }
                Core.Write(writer, value.Expression);
            }
        }


        private static void WriteTypeParameters(OutputWriter writer, string typeParameters,
            InvocationExpressionSyntax invoke)
        {
            if (typeParameters != null)
                writer.Write(typeParameters);
        }

        private static bool IsParamsArgument(InvocationExpressionSyntax invocationExpression, ArgumentSyntax argumentOpt,
            IMethodSymbol methodSymbol)
        {
            if (argumentOpt == null)
                return false;

            if (invocationExpression.ArgumentList.Arguments.Any(o => o.NameColon != null))
                return false; //params cannot be used with named arguments

            int i = invocationExpression.ArgumentList.Arguments.IndexOf(argumentOpt);
            return methodSymbol.Parameters.ElementAt(i).IsParams;
        }

        /// <summary>
        ///     calls to Enum.Parse get re-written as calls to our special Parse methods on each enum.  We assume the first
        ///     parameter to Enum.Parse is a a typeof()
        /// </summary>
        private static void WriteEnumParse(OutputWriter writer, InvocationExpressionSyntax invocationExpression)
        {
            var args = invocationExpression.ArgumentList.Arguments;

            if (args.Count < 2 || args.Count > 3)
                throw new Exception("Expected 2-3 args to Enum.Parse");

            if (args.Count == 3 &&
                (!(args[2].Expression is LiteralExpressionSyntax) ||
                 args[2].Expression.As<LiteralExpressionSyntax>().ToString() != "false"))
            {
                throw new NotImplementedException("Case-insensitive Enum.Parse is not supported " +
                                                  Utility.Descriptor(invocationExpression));
            }

            if (!(args[0].Expression is TypeOfExpressionSyntax))
            {
                throw new Exception("Expected a typeof() expression as the first parameter of Enum.Parse " +
                                    Utility.Descriptor(invocationExpression));
            }

            var type = TypeProcessor.GetTypeInfo(args[0].Expression.As<TypeOfExpressionSyntax>().Type).Type;
            //ModelExtensions.GetTypeInfo(Program.GetModel(invocationExpression), args[0].Expression.As<TypeOfExpressionSyntax>().Type).Type;
            writer.Write(type.ContainingNamespace.FullNameWithDot());
            writer.Write(WriteType.TypeName((INamedTypeSymbol) type));
            writer.Write(".Parse(");
            Core.Write(writer, args[1].Expression);
            writer.Write(")");
        }

        private static void WriteEnumGetValues(OutputWriter writer, InvocationExpressionSyntax invocationExpression)
        {
            if (!(invocationExpression.ArgumentList.Arguments[0].Expression is TypeOfExpressionSyntax))
            {
                throw new Exception("Expected a typeof() expression as the first parameter of Enum.GetValues " +
                                    Utility.Descriptor(invocationExpression));
            }

            //            var type = ModelExtensions.GetTypeInfo(Program.GetModel(invocationExpression), invocationExpression.ArgumentList.Arguments[0].Expression.As<TypeOfExpressionSyntax>().Type).Type;
            var type =
                TypeProcessor.GetTypeInfo(
                    invocationExpression.ArgumentList.Arguments[0].Expression.As<TypeOfExpressionSyntax>().Type).Type;

            writer.Write(type.ContainingNamespace.FullNameWithDot());
            writer.Write(WriteType.TypeName((INamedTypeSymbol) type));
            writer.Write(".Values");
        }
    }
}