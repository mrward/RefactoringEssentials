using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace RefactoringEssentials.CSharp.Diagnostics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RedundantStringToCharArrayCallAnalyzer : DiagnosticAnalyzer
    {
        static readonly DiagnosticDescriptor descriptor = new DiagnosticDescriptor(
            CSharpDiagnosticIDs.RedundantStringToCharArrayCallAnalyzerID,
            GettextCatalog.GetString("Redundant 'string.ToCharArray()' call"),
            GettextCatalog.GetString("Redundant 'string.ToCharArray()' call"),
            DiagnosticAnalyzerCategories.RedundanciesInCode,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            helpLinkUri: HelpLink.CreateFor(CSharpDiagnosticIDs.RedundantStringToCharArrayCallAnalyzerID),
            customTags: DiagnosticCustomTags.Unnecessary
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(descriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(
                (nodeContext) =>
                {
                    Diagnostic diagnostic;
                    if (TryGetDiagnostic(nodeContext, out diagnostic))
                    {
                        nodeContext.ReportDiagnostic(diagnostic);
                    }
                },
                SyntaxKind.InvocationExpression
            );
        }

        static bool TryGetDiagnostic(SyntaxNodeAnalysisContext nodeContext, out Diagnostic diagnostic)
        {
            diagnostic = default(Diagnostic);
            if (nodeContext.IsFromGeneratedCode())
                return false;
            var node = nodeContext.Node as InvocationExpressionSyntax;
            if (node == null)
                return false;
            if (!(node.Parent is ForEachStatementSyntax) && !(node.Parent is ElementAccessExpressionSyntax))
                return false;
            if (!VerifyMethodCalled(node, nodeContext))
                return false;

            var accessExpression = (MemberAccessExpressionSyntax)node.Expression;

            diagnostic = Diagnostic.Create(descriptor, accessExpression.GetLocation());
            return true;
        }

        static bool VerifyMethodCalled(InvocationExpressionSyntax methodCalled, SyntaxNodeAnalysisContext nodeContext)
        {
            if (methodCalled == null)
                throw new ArgumentNullException();
            var expression = methodCalled.Expression as MemberAccessExpressionSyntax;
            var symbol = nodeContext.SemanticModel.GetSymbolInfo(expression).Symbol;
            return symbol.ContainingType.SpecialType == SpecialType.System_String && symbol.Name == "ToCharArray";
        }
    }
}