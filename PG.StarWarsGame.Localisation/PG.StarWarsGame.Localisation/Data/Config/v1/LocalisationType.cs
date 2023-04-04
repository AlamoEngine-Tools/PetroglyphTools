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
    public class LocalisationType : object, System.ComponentModel.INotifyPropertyChanged {
    
        private TranslationType[] m_translationDataField;
    
        private string m_keyField;
    
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("Translation", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public TranslationType[] TranslationData {
            get => m_translationDataField;
            set {
                m_translationDataField = value;
                RaisePropertyChanged("TranslationData");
            }
        }
    
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Key {
            get => m_keyField;
            set {
                m_keyField = value;
                RaisePropertyChanged("Key");
            }
        }
    
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = PropertyChanged;
            propertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}