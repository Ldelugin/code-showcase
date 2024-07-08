using System;
using SoapCore;
using System.Collections.Generic;
using System.ServiceModel.Channels;

namespace -- redacted --;

/// <summary>
/// Description of a SoapEndpoint for automatic registration and
/// easy portability.
/// </summary>
public class SoapEndpointDescriptor
{
    /// <summary>
    /// The class/interface type of the soap service for this endpoint.
    /// </summary>
    public Type ServiceType { get; init; }

    /// <summary>
    /// The SOAP serializer to be used for this endpoint. The default is 
    /// XmlSerializer, which generated and parses Xml using the provided attributes.
    /// </summary>
    public SoapSerializer SoapSerializer { get; init; }

    /// <summary>
    /// The exact path (with leading /) for this soap endpoint, not case sensitive.
    /// </summary>
    public string ServiceMatchPath { get; init; }

    /// <summary>
    /// If not null, a new HTTP method GET handler for the <see cref="ServiceMatchPath"/>
    /// is created, which serves the rawWSDL bytes (as a UTF-8 encoded string).
    /// </summary>
    public byte[] RawWsdl { get; init; }

    /// <summary>
    /// The soap version of the message.
    /// </summary>
    public MessageVersion MessageVersion { get; init; }

    /// <summary>
    /// Set of files and their raw contents that should be served if that file
    /// is requested form the  server. These files will be mapped in the same relative
    /// path as the interface itself.
    /// </summary>
    public Dictionary<string, byte[]> AdditionalFiles { get; } = [];

    /// <summary>
    /// Add an additional file to the SoapEndpointDescriptor.
    /// </summary>
    /// <param name="filename">
    /// The name of the file to be matched.
    /// </param>
    /// <param name="data">
    /// The data of the file in UTF-8 encoded bytes.
    /// </param>
    /// <returns>
    /// SoapEndpointDescriptor instance for fluent api chaining.
    /// </returns>
    public SoapEndpointDescriptor RegisterAdditionalFile(string filename, byte[] data)
    {
        this.AdditionalFiles[filename] = data;
        return this;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="SoapEndpointDescriptor"/> specific for the SOAP 1.2 version.
    /// </summary>
    /// <param name="serviceMatchPath">The exact path (with leading /) for this soap endpoint, not case sensitive.</param>
    /// <param name="rawWsdl">
    /// If not null, a new HTTP method GET handler for the <see cref="ServiceMatchPath"/>
    /// is created, which serves the rawWSDL bytes (as a UTF-8 encoded string).
    /// </param>
    /// <param name="soapSerializer">
    /// The SOAP serializer to be used for this endpoint. The default is 
    /// XmlSerializer, which generated and parses XML using the XML-attributes.
    /// </param>
    /// <typeparam name="T">The class/interface type of the soap service for this endpoint.</typeparam>
    /// <returns>A new instance of <see cref="SoapEndpointDescriptor"/>.</returns>
    public static SoapEndpointDescriptor Soap12<T>(string serviceMatchPath, byte[] rawWsdl = null,
        SoapSerializer soapSerializer = SoapSerializer.XmlSerializer)
        => Create<T>(serviceMatchPath, MessageVersion.Soap12WSAddressingAugust2004, rawWsdl, soapSerializer);

    /// <summary>
    /// Creates a new instance of the <see cref="SoapEndpointDescriptor"/> specific for the SOAP 1.1 version.
    /// </summary>
    /// <param name="serviceMatchPath">The exact path (with leading /) for this soap endpoint, not case sensitive.</param>
    /// <param name="rawWsdl">
    /// If not null, a new HTTP method GET handler for the <see cref="ServiceMatchPath"/>
    /// is created, which serves the rawWSDL bytes (as a UTF-8 encoded string).
    /// </param>
    /// <param name="soapSerializer">
    /// The SOAP serializer to be used for this endpoint. The default is 
    /// XmlSerializer, which generated and parses XML using the XML-attributes.
    /// </param>
    /// <typeparam name="T">The class/interface type of the soap service for this endpoint.</typeparam>
    /// <returns>A new instance of <see cref="SoapEndpointDescriptor"/>.</returns>
    public static SoapEndpointDescriptor Soap11<T>(string serviceMatchPath, byte[] rawWsdl = null,
        SoapSerializer soapSerializer = SoapSerializer.XmlSerializer)
        => Create<T>(serviceMatchPath, MessageVersion.Soap11, rawWsdl, soapSerializer);

    /// <summary>
    /// Creates a new instance of the <see cref="SoapEndpointDescriptor"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The class/interface type of the soap service for this endpoint.
    /// </typeparam>
    /// <param name="serviceMatchPath">
    /// The exact path (with leading /) for this soap endpoint, not case sensitive.
    /// </param>
    /// <param name="messageVersion">The soap version of the message.</param>
    /// <param name="rawWsdl">
    /// If not null, a new HTTP method GET handler for the <see cref="ServiceMatchPath"/>
    /// is created, which serves the rawWSDL bytes (as a UTF-8 encoded string).
    /// </param>
    /// <param name="soapSerializer">
    /// The SOAP serializer to be used for this endpoint. The default is 
    /// XmlSerializer, which generated and parses XML using the XML-attributes.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="SoapEndpointDescriptor"/>.
    /// </returns>
    private static SoapEndpointDescriptor Create<T>(string serviceMatchPath, MessageVersion messageVersion,
        byte[] rawWsdl = null, SoapSerializer soapSerializer = SoapSerializer.XmlSerializer)
    {
        return new SoapEndpointDescriptor
        {
            ServiceType = typeof(T),
            SoapSerializer = soapSerializer,
            ServiceMatchPath = serviceMatchPath,
            RawWsdl = rawWsdl,
            MessageVersion = messageVersion
        };
    }
}

