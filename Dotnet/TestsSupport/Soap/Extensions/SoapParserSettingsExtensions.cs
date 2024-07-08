using System.Collections.Generic;

namespace TestsSupport.Soap.Extensions;

/// <summary>
/// Contains extension methods for <see cref="SoapParserSettings"/>.
/// </summary>
public static class SoapParserSettingsExtensions
{
    /// <summary>
    /// With the provided <see cref="SoapVersion"/> set on the <see cref="SoapParserSettings"/>.
    /// </summary>
    /// <param name="settings">
    /// The <see cref="SoapParserSettings"/> to add the <paramref name="soapVersion"/> to.
    /// </param>
    /// <param name="soapVersion">
    /// The <see cref="SoapParserSettings.SoapVersion"/> to add to the <see cref="SoapParserSettings"/>.
    /// </param>
    /// <returns>The <see cref="SoapParserSettings"/>.</returns>
    public static SoapParserSettings WithSoapVersion(this SoapParserSettings settings, SoapVersion soapVersion)
    {
        settings.SoapVersion = soapVersion;
        return settings;
    }

    /// <summary>
    /// With the provided <see cref="SoapParserSettings.OmitBodyContent"/> set on the <see cref="SoapParserSettings"/>.
    /// </summary>
    /// <param name="settings">
    /// The <see cref="SoapParserSettings"/> to add the <paramref name="omit"/> to.
    /// </param>
    /// <returns>The <see cref="SoapParserSettings"/>.</returns>
    public static SoapParserSettings WithOmitBodyContent(this SoapParserSettings settings)
    {
        settings.OmitBodyContent = true;
        return settings;
    }

    /// <summary>
    /// With the provided <see cref="SoapParserSettings.PartiallyOmitBodyContent"/> set on the <see cref="SoapParserSettings"/>.
    /// </summary>
    /// <param name="settings">
    /// The <see cref="SoapParserSettings"/> to add the <paramref name="omit"/> to.
    /// </param>
    /// <returns>The <see cref="SoapParserSettings"/>.</returns>
    public static SoapParserSettings WithPartiallyOmitBodyContent(this SoapParserSettings settings)
    {
        settings.PartiallyOmitBodyContent = true;
        return settings;
    }

    /// <summary>
    /// With the provided <see cref="SoapParserSettings.OmitEnvelopeContent"/> set on the <see cref="SoapParserSettings"/>.
    /// </summary>
    /// <param name="settings">
    /// The <see cref="SoapParserSettings"/> to add the <paramref name="omitEnvelopeContent"/> to.
    /// </param>
    /// <param name="overridePair">
    /// The key value pair of the element to override and the value to override it with (e.g. "//int:Limit" "abc")
    /// </param>
    /// <returns>The <see cref="SoapParserSettings"/>.</returns>
    public static SoapParserSettings WithOverride(this SoapParserSettings settings, KeyValuePair<string, string> overridePair)
    {
        settings.Overrides ??= [];
        settings.Overrides.Add(overridePair.Key, overridePair.Value);
        return settings;
    }

    /// <summary>
    /// With the provided <see cref="SoapParserSettings.OmitEnvelopeContent"/> set on the <see cref="SoapParserSettings"/>.
    /// </summary>
    /// <param name="settings">
    /// The <see cref="SoapParserSettings"/> to add the <paramref name="elementToShuffle"/> to.
    /// </param>
    /// <param name="elementToShuffle">
    /// The element to shuffle (e.g. "//ser:Passage")
    /// </param>
    /// <returns>The <see cref="SoapParserSettings"/>.</returns>
    public static SoapParserSettings WithElementToShuffle(this SoapParserSettings settings, string elementToShuffle)
    {
        settings.ElementsToShuffle ??= [];
        settings.ElementsToShuffle.Add(elementToShuffle);
        return settings;
    }

    /// <summary>
    /// With the provided <see cref="SoapParserSettings.OmitEnvelopeContent"/> set on the <see cref="SoapParserSettings"/>.
    /// </summary>
    /// <param name="settings">
    /// The <see cref="SoapParserSettings"/> to add the <paramref name="additionalElementToAdd"/> to.
    /// </param>
    /// <param name="additionalElementToAdd">
    /// The key value pair of the element to add and the value to add it with (e.g. "//int:-- redacted --" new Dictionary{string string} { { "AdditionalElement" "AdditionalElementValue" } })
    /// </param>
    /// <returns>The <see cref="SoapParserSettings"/>.</returns>
    public static SoapParserSettings WithAdditionalElementToAdd(this SoapParserSettings settings, KeyValuePair<string, Dictionary<string, string>> additionalElementToAdd)
    {
        settings.AdditionalElementsToAdd ??= [];
        settings.AdditionalElementsToAdd.Add(additionalElementToAdd.Key, additionalElementToAdd.Value);
        return settings;
    }

    /// <summary>
    /// With the provided <see cref="SoapParserSettings.OmitEnvelopeContent"/> set on the <see cref="SoapParserSettings"/>.
    /// </summary>
    /// <param name="settings">
    /// The <see cref="SoapParserSettings"/> to add the <paramref name="additionalAttributeToAdd"/> to.
    /// </param>
    /// <param name="additionalAttributeToAdd">
    /// The key value pair of the attribute to add and the value to add it with (e.g. "//int:-- redacted --" new Dictionary{string string} { { "AdditionalAttribute" "AdditionalAttributeValue" } })
    /// </param>
    /// <returns>The <see cref="SoapParserSettings"/>.</returns>
    public static SoapParserSettings WithAdditionalAttributeToAdd(this SoapParserSettings settings, KeyValuePair<string, Dictionary<string, string>> additionalAttributeToAdd)
    {
        settings.AdditionalAttributesToAdd ??= [];
        settings.AdditionalAttributesToAdd.Add(additionalAttributeToAdd.Key, additionalAttributeToAdd.Value);
        return settings;
    }
}