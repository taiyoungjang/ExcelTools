using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;


/// <summary>
/// Extensions methods for xml doc files
/// </summary>
public static class XmlDocHelper
{
    #region Public Methods

    /// <summary>
    /// Get the xml element representing the member
    /// </summary>
    /// <param name="membersRoot"></param>
    /// <param name="member"></param>
    /// <returns></returns>
    public static XElement GetDocMember(XElement membersRoot, MemberInfo member)
    {
        string memberId = GetMemberId(member);

        return membersRoot
            .Elements("member")
            .FirstOrDefault(e => e.Attribute("name")?.Value == memberId);
    }

    /// <summary>
    /// Get the xml element representing the member
    /// </summary>
    /// <param name="membersRoot"></param>
    /// <param name="member"></param>
    /// <returns></returns>
    public static XElement GetTypeMember(XElement membersRoot, TypeInfo member)
    {
        string memberId = GetMemberId(member);

        return membersRoot
            .Elements("member")
            .First(e => e.Attribute("name")?.Value == memberId);
    }

    /// <summary>
    /// Find the XML documentation files for a given assembly
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public static FileInfo GetXmlDocFile(Assembly assembly, CultureInfo culture = null)
    {
        var fileName = Path.GetFileNameWithoutExtension(assembly.Location) + ".xml";

        return EnumeratePossibleXmlDocumentationLocation(assembly, culture ?? CultureInfo.CurrentCulture)
                   .Select(directory => Path.Combine(directory, fileName))
                   .Select(filePath => new FileInfo(filePath))
                   .FirstOrDefault(file => file.Exists)
               ?? throw new ArgumentException($"No XML doc file found for assembly '{assembly.FullName}'");
    }

    #endregion Public Methods

    #region Private Methods

    private static IEnumerable<string> EnumeratePossibleXmlDocumentationLocation(Assembly assembly, CultureInfo culture)
    {
        var currentCulture = culture;

        var locations = new[] { new FileInfo(assembly.Location)?.Directory?.FullName, AppContext.BaseDirectory };

        foreach (var location in locations)
        {
            while (currentCulture.Name != CultureInfo.InvariantCulture.Name)
            {
                yield return Path.Combine(location, currentCulture.Name);
                currentCulture = currentCulture.Parent;
            }

            yield return Path.Combine(location, String.Empty);
        }
    }

    private static string GetMemberFullName(MemberInfo member)
    {
        var memberScope = "";

        if (member.DeclaringType != null)
            memberScope = GetMemberFullName(member.DeclaringType);
        else if (member is Type type)
            memberScope = type.Namespace;

        if (string.IsNullOrEmpty(memberScope))
            return member.Name;

        return $"{memberScope}.{member.Name}";
    }

    private static string GetMemberId(MemberInfo member)
    {
        var memberKindPrefix = GetMemberPrefix(member);
        var memberName = GetMemberFullName(member);

        return $"{memberKindPrefix}:{memberName}";
    }

    private static char GetMemberPrefix(MemberInfo member)
    {
        var typeName = member.GetType().Name;

        switch (typeName)
        {
            case "MdFieldInfo": return 'F';

            default:
                return typeName.Replace("Runtime", "")[0];
        }
    }

    #endregion Private Methods
}
