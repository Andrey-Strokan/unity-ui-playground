using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

/// <summary>
/// Helper utility for generating C# sources. Basically it is a extension to StringBuilder
/// which handles indentation of the source code.
/// </summary>
public class CodeGeneratorUtility
{
    private StringBuilder builder = new StringBuilder();
    private int indentLevel = 0;

    /// <summary>
    /// Increment indentation level.
    /// </summary>
    public void Indent()
    {
        indentLevel++;
    }

    /// <summary>
    /// Decrement indentation level.
    /// </summary>
    public void Unindent()
    {
        if (indentLevel > 0)
        {
            indentLevel--;
        }
    }

    /// <summary>
    /// Same as StringBuilder AppendFormat but adds indentation spaces at the beginning.
    /// </summary>
    public CodeGeneratorUtility AppendFormat (string format, params object[] args)
    {
        AppendIndent();
        builder.AppendFormat(format, args);
        return this;
    }

    /// <summary>
    /// Same as StringBuilder Append methods but adds indentation spaces at the beginning.
    /// </summary>
    public CodeGeneratorUtility Append<T>(T arg)
    {
        AppendIndent();
        builder.Append(arg);
        return this;
    }

    /// <summary>
    /// Same as StringBuilder AppendLine but adds indentation spaces at the beginning.
    /// </summary>
    public CodeGeneratorUtility AppendLine()
    {
        builder.AppendLine();
        return this;
    }

    /// <summary>
    /// Same as StringBuilder AppendLine but adds indentation spaces at the beginning.
    /// </summary>
    public CodeGeneratorUtility AppendLine(string str)
    {
        AppendIndent();
        builder.AppendLine(str);
        return this;
    }

    public override string ToString()
    {
        return builder.ToString();
    }

    private void AppendIndent()
    {
        for (int i = 0; i < indentLevel; i++)
        {
            builder.Append("    ");
        }
    }
}
