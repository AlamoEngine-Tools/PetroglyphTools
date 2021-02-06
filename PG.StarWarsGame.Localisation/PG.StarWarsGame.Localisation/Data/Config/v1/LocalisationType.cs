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
    public class LocalisationType : object, INotifyPropertyChanged
    {
        private string m_keyField;

        private TranslationType[] m_translationDataField;

        /// <remarks />
        [XmlArray(Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("Translation", Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public TranslationType[] TranslationData
        {
            get => m_translationDataField;
            set
            {
                m_translationDataField = value;
                RaisePropertyChanged("TranslationData");
            }
        }

        /// <remarks />
        [XmlAttribute]
        public string Key
        {
            get => m_keyField;
            set
            {
                m_keyField = value;
                RaisePropertyChanged("Key");
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
