using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

public class EnumDescriptorSchemaFilter 
{
    #region Public Methods

    /// <summary>
    ///
    /// </summary>
    /// <param name="schema"></param>
    /// <param name="context"></param>
    // public void Apply(Schema schema, SchemaFilterContext context)
    // {
    //     var typeInfo = context.SystemType.GetTypeInfo();
    //
    //     if (typeInfo.IsEnum)
    //     {
    //         schema.Description = BuildDescription(typeInfo);
    //     }
    // }

    #endregion Public Methods

    #region Private Methods

    public static Dictionary<string,string> BuildDescription(TypeInfo typeInfo)
    {
        var docMembers = LoadXmlMembers(typeInfo);
        return BuildMembersDescription(typeInfo, docMembers);
    }

    private static Dictionary<string,string> BuildMembersDescription(TypeInfo typeInfo, XElement docMembers)
    {
        Dictionary<string, string> dic = new();
        var enumValues = Enum.GetValues(typeInfo);

        for (int i = 0; i < enumValues.Length; i++)
        {
            var enumValue = enumValues.GetValue(i);
            var member = typeInfo.GetMember(enumValue.ToString()).Single();
            var docMember = XmlDocHelper.GetDocMember(docMembers, member);
            if (docMember == null)
            {
                continue;
            }
            var name = enumValue.ToString();
            var description = docMember.Value.Trim();

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(description))
            {
                dic.Add(name,description);
            }
        }
        return dic;
    }

    private static XElement LoadXmlMembers(TypeInfo typeInfo)
    {
        var file = XmlDocHelper.GetXmlDocFile(typeInfo.Assembly);
        var docXml = XDocument.Load(file.FullName);
        var xmlRoot = docXml.Root;

        if (xmlRoot == null) throw new ArgumentNullException(nameof(xmlRoot) + ", for " + typeInfo.FullName);

        return xmlRoot.Element("members");
    }

    #endregion Private Methods
}
