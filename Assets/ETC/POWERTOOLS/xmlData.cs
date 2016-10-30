using UnityEngine;
using System.Collections;
using System.Xml; //  xml 추가
using System.Xml.Serialization;//  xml 추가
using System.Collections.Generic;
using System.Text;
using System.IO;

public class xmlData {

    [XmlElement("#")]
    public string number = null;

    [XmlElement("Description")]
    public string description = null;

    [XmlElement("Start Date")]
    public string startTime = null;

    [XmlElement("Complete Date")]
    public string completeTime = null;

    [XmlElement("Elapsed Time")]
    public string elapsedTime = null;

    [XmlElement("Detailed Contents")]
    public string detailContents = null;


}

[XmlRoot("data-set")]
public class Root
{
    [XmlElement("record")]
    public xmlData[] RecordXmlData;
}