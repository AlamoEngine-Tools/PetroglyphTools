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
    /// <remarks/>
    [ExcludeFromCodeCoverage]
    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "urn:alamoenginetools:localisation:v1")]
    [XmlRoot("LocalisationData", Namespace = "urn:alamoenginetools:localisation:v1", IsNullable = false)]
    public class LocalisationDataType : object, INotifyPropertyChanged
    {
        private LocalisationType[]? m_localisationField;

        /// <remarks/>
        [XmlElement("Localisation", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public LocalisationType[]? Localisation
        {
            get => m_localisationField;
            set
            {
                m_localisationField = value;
                RaisePropertyChanged("Localisation");
            }
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
