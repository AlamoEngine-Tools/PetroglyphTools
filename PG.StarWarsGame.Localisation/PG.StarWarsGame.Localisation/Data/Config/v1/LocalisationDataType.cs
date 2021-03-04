// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace PG.StarWarsGame.Localisation.Data.Config.v1
{
    /// <remarks />
    [ExcludeFromCodeCoverage]
    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.example.org/eaw-translation/")]
    [XmlRoot("LocalisationData", Namespace = "http://www.example.org/eaw-translation/", IsNullable = false)]
    public class LocalisationDataType : object, INotifyPropertyChanged
    {
        private LocalisationType[] m_localisationField;

        /// <remarks />
        [XmlElement("Localisation", Form = XmlSchemaForm.Unqualified)]
        public LocalisationType[] Localisation
        {
            get => m_localisationField;
            set
            {
                m_localisationField = value;
                RaisePropertyChanged("Localisation");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
