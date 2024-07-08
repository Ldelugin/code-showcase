export default function Log (log, existingServices) {
    this.id = log.id;
    this.message = log.message;
    this.serviceName = getServiceName(log.serviceName, existingServices);
    this.processId = log.processId;
    this.logLevel = getLogLevelName(log.logLevel);
    this.timestamp = log.timestamp;
    this._showDetails = false;
    
    function getLogLevelName(logLevel) {
        switch (logLevel) {
            case 0: return 'Trace';
            case 1: return 'Debug';
            case 2: return 'Information';
            case 3: return 'Warning';
            case 4: return 'Error';
            case 5: return 'Critical';
            default: return 'Unknown';
        }
    }
    
    function getServiceName(service, allServices) {
        return allServices[service];
    }
}