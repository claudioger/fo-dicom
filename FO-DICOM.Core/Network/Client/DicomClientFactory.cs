﻿using System;
using FellowOakDicom.Imaging.Codec;
using FellowOakDicom.Log;
using Microsoft.Extensions.Options;

namespace FellowOakDicom.Network.Client
{
    public interface IDicomClientFactory
    {
        /// <summary>
        /// Initializes an instance of <see cref="DicomClient"/>.
        /// </summary>
        /// <param name="host">DICOM host.</param>
        /// <param name="port">Port.</param>
        /// <param name="useTls">True if TLS security should be enabled, false otherwise.</param>
        /// <param name="callingAe">Calling Application Entity Title.</param>
        /// <param name="calledAe">Called Application Entity Title.</param>
        /// <param name="configureClientOptions">An optional modifier that can change the client options of this DICOM client</param>
        /// <param name="configureServiceOptions">An optional modifier action that can change the service options of this DICOM client</param>
        IDicomClient Create(string host, int port, bool useTls, string callingAe, string calledAe, 
            Action<DicomClientOptions> configureClientOptions = null,
            Action<DicomServiceOptions> configureServiceOptions = null);
    }

    public class DicomClientFactory : IDicomClientFactory
    {
        private readonly ILogManager _logManager;
        private readonly INetworkManager _networkManager;
        private readonly ITranscoderManager _transcoderManager;
        private readonly IOptions<DicomClientOptions> _defaultClientOptions;
        private readonly IOptions<DicomServiceOptions> _defaultServiceOptions;

        public DicomClientFactory(
            ILogManager logManager, INetworkManager networkManager, ITranscoderManager transcoderManager,
            IOptions<DicomClientOptions> defaultClientOptions,
            IOptions<DicomServiceOptions> defaultServiceOptions
            )
        {
            _logManager = logManager ?? throw new ArgumentNullException(nameof(logManager));
            _networkManager = networkManager ?? throw new ArgumentNullException(nameof(networkManager));
            _transcoderManager = transcoderManager ?? throw new ArgumentNullException(nameof(transcoderManager));
            _defaultClientOptions = defaultClientOptions ?? throw new ArgumentNullException(nameof(defaultClientOptions));
            _defaultServiceOptions = defaultServiceOptions ?? throw new ArgumentNullException(nameof(defaultServiceOptions));
        }

        public IDicomClient Create(string host, int port, bool useTls, string callingAe, string calledAe,
            Action<DicomClientOptions> configureClientOptions = null,
            Action<DicomServiceOptions> configureServiceOptions = null)
        {
            var clientOptions = _defaultClientOptions.Value.Clone();
            var serviceOptions = _defaultServiceOptions.Value.Clone();

            configureClientOptions?.Invoke(clientOptions);
            configureServiceOptions?.Invoke(serviceOptions);

            return new DicomClient(host, port, useTls, callingAe, calledAe, clientOptions, serviceOptions, _networkManager, _logManager, _transcoderManager);
        }
    }
}