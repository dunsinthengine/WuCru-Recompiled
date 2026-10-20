using System;
using System.Xml;

namespace AmuzoEngine
{
	public static class XmlUtils
	{
		public static void SetXmlChild(XmlElement parentElem, string childName, Action<XmlElement> setAction)
		{
			SetXmlChild(parentElem, (XmlElement e) => e.Name == childName, () => parentElem.OwnerDocument.CreateElement(childName), setAction);
		}

		public static void SetXmlChild(XmlElement parentElem, Predicate<XmlElement> findPred, Func<XmlElement> createFunc, Action<XmlElement> setAction)
		{
			XmlElement xmlElement = FindXmlChild(parentElem, findPred);
			if (xmlElement != null)
			{
				if (setAction != null)
				{
					setAction(xmlElement);
				}
			}
			else
			{
				AddXmlChild(parentElem, createFunc, setAction);
			}
		}

		public static void RemoveXmlChild(XmlElement parentElem, string childName)
		{
			RemoveXmlChild(parentElem, (XmlElement e) => e.Name == childName);
		}

		public static void RemoveXmlChild(XmlElement parentElem, Predicate<XmlElement> findPred)
		{
			XmlElement xmlElement = FindXmlChild(parentElem, findPred);
			if (xmlElement != null)
			{
				parentElem.RemoveChild(xmlElement);
			}
		}

		public static XmlElement FindXmlChild(XmlElement parentElem, Predicate<XmlElement> match)
		{
			XmlNodeList xmlNodeList = parentElem.SelectNodes("child::node()");
			XmlElement result = null;
			if (xmlNodeList != null)
			{
				int count = xmlNodeList.Count;
				for (int i = 0; i < count; i++)
				{
					XmlNode xmlNode = xmlNodeList[i];
					if (xmlNode != null)
					{
						XmlElement xmlElement = xmlNode as XmlElement;
						if (xmlElement != null && (match == null || match(xmlElement)))
						{
							result = xmlElement;
							break;
						}
					}
				}
			}
			return result;
		}

		public static XmlElement FindXmlElement(XmlElement parentElem, string elementPath)
		{
			if (string.IsNullOrEmpty(elementPath))
			{
				return null;
			}
			string[] array = elementPath.Split('/', '\\');
			XmlElement xmlElement = parentElem;
			string[] array2 = array;
			foreach (string node in array2)
			{
				xmlElement = FindXmlChild(xmlElement, (XmlElement e) => e.Name.Equals(node, StringComparison.OrdinalIgnoreCase));
				if (xmlElement == null)
				{
					break;
				}
			}
			return xmlElement;
		}

		public static void AddXmlChild(XmlElement parentElem, string childName, Action<XmlElement> setAction)
		{
			AddXmlChild(parentElem, () => parentElem.OwnerDocument.CreateElement(childName, parentElem.NamespaceURI), setAction);
		}

		public static void AddXmlChild(XmlElement parentElem, Func<XmlElement> createFunc, Action<XmlElement> setAction)
		{
			XmlElement xmlElement = createFunc();
			if (xmlElement == null)
			{
				throw new Exception("Failed to create child element");
			}
			parentElem.AppendChild(xmlElement);
			if (setAction != null)
			{
				setAction(xmlElement);
			}
		}
	}
}
