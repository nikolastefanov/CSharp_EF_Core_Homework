using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Xml.Serialization;

namespace TeisterMask.DataProcessor.ExportDto
{
    [XmlType("Task")]
    public class ExportProjectTaskDto
    {
        [XmlElement("Name")]
        public string Name { get; set; }


        [XmlElement("Label")]
        public string LabelType { get; set; }

        [XmlArray("Tasks")]
        public virtual ExportProjectTaskDto[] Tasks { get; set; }
    }
}
