// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.Localisation.Data.Config.v1
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.example.org/eaw-translation/")]
    [System.Xml.Serialization.XmlRootAttribute("LocalisationData", Namespace="http://www.example.org/eaw-translation/", IsNullable=false)]
    public class LocalisationDataType : object, System.ComponentModel.INotifyPropertyChanged {
    
        private LocalisationType[] m_localisationField;
    
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Localisation", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public LocalisationType[] Localisation {
            get => m_localisationField;
            set {
                m_localisationField = value;
                RaisePropertyChanged("Localisation");
            }
        }
    
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = PropertyChanged;
            propertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}