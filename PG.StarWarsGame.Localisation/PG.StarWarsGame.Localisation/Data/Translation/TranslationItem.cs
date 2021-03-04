// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using PG.Core.Data;
using PG.Core.Localisation;

namespace PG.StarWarsGame.Localisation.Data.Translation
{
    /// <summary>
    /// The versioned representation of an Alamo Engine localisation entry, consisting of a
    /// <see cref="Key"/>, <see cref="Value"/> and <see cref="Language"/>. 
    /// </summary>
    public sealed class TranslationItem : INotifyPropertyChanged, IPersistenceEntity
    {
        private readonly string m_missingValue;
        private string m_key;
        private readonly IDictionary<long, string> m_valueHistory;
        private long m_version;
        private bool m_isTodoItem;

        public TranslationItem([JetBrains.Annotations.NotNull] string key,
            [JetBrains.Annotations.NotNull] IAlamoLanguageDefinition language,
            [CanBeNull] string value = null)
        {
            m_valueHistory = new Dictionary<long, string>();
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Language = language ?? throw new ArgumentNullException(nameof(language));
            m_missingValue =
                $"WARNING - [{Language.LanguageIdentifier.ToUpper()}] - Missing translation for {nameof(key)}: \"{Key}\"";
            Value = value;
        }

        /// <summary>
        /// The key identifying the translation item. Always upper case.
        /// </summary>
        public string Key
        {
            get => m_key;
            private set => m_key = value.ToUpper();
        }

        /// <summary>
        /// The versioned value of the translation item. If set to null it will be automatically replaced with a
        /// detailed warning message and marked as a <see cref="IsTodoItem"/>.
        /// </summary>
        public string Value
        {
            get => m_valueHistory[Version];
            set
            {
                if (Version > 0 && (string.Compare(m_valueHistory[Version], value, false, Language.Culture) == 0
                                    || string.Compare(m_missingValue, value, false, Language.Culture) == 0))
                {
                    return;
                }

                Version++;
                if (value == null)
                {
                    m_valueHistory[Version] = m_missingValue;
                    IsTodoItem = true;
                }
                else
                {
                    m_valueHistory[Version] = value;
                    IsTodoItem = false;
                }

                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// The latest version number of the translation value.
        /// </summary>
        public long Version
        {
            get => m_version;
            private set
            {
                m_version = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Accessor to the change history of the <see cref="TranslationItem"/>'s <see cref="Value"/>. 
        /// </summary>
        /// <param name="version">The desired version of the <see cref="TranslationItem"/>'s <see cref="Value"/>.
        /// Should be between 1 and <see cref="TranslationItem"/>.<see cref="Version"/></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown if an invalid version is provided.</exception>
        public string GetValueByVersion(long version)
        {
            if (version <= 0 || version > Version)
            {
                throw new ArgumentException(
                    $"The provided version {version} does not exist. Accepted version range: [1,{Version}]",
                    nameof(version));
            }

            return m_valueHistory[version];
        }

        /// <summary>
        /// Indexer for the Value property. Equivalent to calling <see cref="GetValueByVersion"/>.
        /// </summary>
        /// <param name="i">The desired version of the <see cref="TranslationItem"/>'s <see cref="Value"/>.
        /// Should be between 1 and <see cref="TranslationItem"/>.<see cref="Version"/></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown if an invalid version is provided.</exception>
        public string this[long i] => GetValueByVersion(i);


        /// <summary> 
        /// Safe accessor to the change history of the <see cref="TranslationItem"/>'s <see cref="Value"/>.
        /// </summary>
        /// <param name="version">The desired version of the <see cref="TranslationItem"/>'s <see cref="Value"/>.</param>
        /// <param name="value">Set to the <see cref="TranslationItem"/>'s <see cref="Value"/> if present.</param>
        /// <returns>Returns true, if a value was found, false else.</returns>
        public bool TryGetValueByVersion(long version, out string value)
        {
            value = null;
            try
            {
                value = GetValueByVersion(version);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        /// <summary>
        /// The <see cref="IAlamoLanguageDefinition"/> associated with the <see cref="TranslationItem"/>.
        /// </summary>
        public IAlamoLanguageDefinition Language { get; }


        /// <summary>
        /// A bool flag which is set to true if there is a reasonable concern that the <see cref="TranslationItem"/>'s
        /// value is not a proper translation.
        /// </summary>
        public bool IsTodoItem
        {
            get => m_isTodoItem;
            private set
            {
                if (m_isTodoItem == value)
                {
                    return;
                }

                m_isTodoItem = value;
                NotifyPropertyChanged();
            }
        }

        #region Auto-generated Equality Comparer

        [ExcludeFromCodeCoverage]
        public bool Equals(TranslationItem other)
        {
            if (other == null)
            {
                return false;
            }

            return GetHashCode() == other.GetHashCode();
        }


        [ExcludeFromCodeCoverage]
        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is TranslationItem other && Equals(other);
        }


        [ExcludeFromCodeCoverage]
        public override int GetHashCode()
        {
            return $"K[{Key.ToUpper()}:{Version}]-L[{Language.LanguageIdentifier.ToUpper()}]"
                .GetHashCode();
        }

        [ExcludeFromCodeCoverage]
        public static bool operator ==(TranslationItem left, TranslationItem right)
        {
            return Equals(left, right);
        }


        [ExcludeFromCodeCoverage]
        public static bool operator !=(TranslationItem left, TranslationItem right)
        {
            return !Equals(left, right);
        }

        #endregion

        #region Auto-generated INotifyProprtyChanged

        [ExcludeFromCodeCoverage] public event PropertyChangedEventHandler PropertyChanged;

        [ExcludeFromCodeCoverage]
        [NotifyPropertyChangedInvocator]
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public string Id => Key;
    }
}
