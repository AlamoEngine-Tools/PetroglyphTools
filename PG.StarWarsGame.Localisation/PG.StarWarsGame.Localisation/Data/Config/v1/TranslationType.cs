// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
    public class TranslationType : object, INotifyPropertyChanged
    {
        private string m_languageField;
        private string m_valueField;

        /// <remarks />
        [XmlAttribute]
        public string Language
        {
            get => m_languageField;
            set
            {
                m_languageField = value;
                RaisePropertyChanged("Language");
            }
        }

        /// <remarks />
        [XmlText]
        public string Value
        {
            get => m_valueField;
            set
            {
                m_valueField = value;
                RaisePropertyChanged("Value");
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
