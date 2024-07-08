<template>
    <div id="LiveLogView">
        <b-container class="p-3">
            <b-row>                 
                <b-col class="mr-auto lead mb-1">
                    <h2><strong>Live Log View</strong></h2>
                </b-col>
                
                <b-col cols="col-auto">
                    <b-spinner v-show="isLoading" type="grow" small label="Loading..."></b-spinner>
                </b-col>
                
                <b-col class="col-auto">
                    <b-button-toolbar>
                        <b-button-group>
                            <b-button 
                                :variant="refreshEnabled ? 'success' : 'outline-secondary'" 
                                onclick="this.blur();" 
                                @click="togglePause()"
                                v-b-tooltip.hover title="New logs are automatically added when Refresh is enabled."
                            >
                                <font-awesome-icon icon="pause-circle" v-show="refreshEnabled"></font-awesome-icon>
                                <font-awesome-icon icon="play-circle" v-show="!refreshEnabled"></font-awesome-icon>
                                Refresh
                            </b-button>
                            
                            <b-button 
                                :variant="showFilters ? 'secondary' : 'outline-secondary'" 
                                @click="showFilters = !showFilters"
                                v-b-tooltip.hover title="Show/Hide the filters menu."
                            >
                                <font-awesome-icon icon="filter"></font-awesome-icon>
                                {{ showFilters ? 'Hide' : 'Show' }} filters
                            </b-button>
                            
                            <b-button 
                                variant="secondary" 
                                @click="sortDescending = !sortDescending"
                                v-b-tooltip.hover title="Toggle between showing the newest log first or last."
                            >
                                <font-awesome-icon icon="sort-down" v-show="!sortDescending"></font-awesome-icon>
                                <font-awesome-icon icon="sort-up" v-show="sortDescending"></font-awesome-icon>
                                Sort {{ sortDescending ? 'descending' : 'ascending' }}
                            </b-button>
                            
                        </b-button-group>
                    </b-button-toolbar>
                </b-col>
            </b-row>

            <b-collapse id="applied-filters-collapse" v-model="showAppliedFilters">

                <b-row>

                    <b-col cols="10">
                        <b-form-tags
                            v-model="appliedFilters"
                            size="sm"
                            tag-variant="primary"
                        >
                            <template v-slot="{ tags, tagVariant, removeTag }">
                                <b-form-tag
                                    v-for="tag in tags"
                                    @remove="onRemoveTag({tag, removeTag})"
                                    :key="tag"
                                    :title="tag"
                                    :variant="tagVariant"
                                    class="mr-1"
                                >{{ tag }}</b-form-tag>
                            </template>

                        </b-form-tags>
                    </b-col>

                    <b-col cols="col-auto">
                        <b-button
                            variant="link"
                            @click="onClearAllAppliedFilters"
                            size="sm"
                            v-b-tooltip.hover title="Clear all the applied filters."
                        >Clear all applied filters</b-button>
                    </b-col>

                </b-row>

            </b-collapse>
            
            <b-row>
                
                <b-col>
                    <b-collapse id="filter-collapse" v-model="showFilters">
                    
                    <b-card bg-variant="light" no-body class="shadow-sm">
                        <b-form-group
                            label="Filters"
                            label-cols-lg="1"
                            label-size="lg"
                            label-class="font-weight-bold pt-0"
                            class="mb-0"
                        >

                            <b-form-group
                                label="Search:"
                                label-for="filter-search"
                                label-cols-sm="3"
                                label-align-sm="right"
                                label-size="sm"
                                label-class="font-weight-bold pt-0"
                                description="Search on the full log message."
                                class="mb-0"
                            >
                                <b-input-group size="sm">
                                    <b-form-input
                                        id="filter-search"
                                        v-model="textSearchFilter"
                                        type="search"
                                        placeholder="Type to Search"
                                    ></b-form-input>

                                    <b-input-group-append>
                                        <b-button :disabled="!textSearchFilter" @click="textSearchFilter = ''">Clear</b-button>
                                    </b-input-group-append>
                                </b-input-group>
                            </b-form-group>

                            <b-form-group
                                label="Services"
                                label-for="filter-service-names"
                                label-cols-sm="3"
                                label-align-sm="right"
                                label-size="sm"
                                label-class="font-weight-bold pt-0"
                                description="Select the service(s) that should be included."
                                class="mb-0"
                            >

                                <b-form-select
                                    id="filter-service-names"
                                    v-model="serviceNames"
                                    :options="optionsForServiceNames"
                                    multiple
                                    :select-size="4"></b-form-select>
                            </b-form-group>
                            
                            <b-form-group
                                label="ProcessIds"
                                label-for="filter-process-ids"
                                label-cols-sm="3"
                                label-align-sm="right"
                                label-size="sm"
                                label-class="font-weight-bold pt-0"
                                description="Select the processId(s) that should be included."
                                class="mb-0"
                            >

                                <b-form-tags
                                    id="filter-process-ids"
                                    v-model="processIds"
                                    size="sm"
                                    tag-variant="primary"
                                >
                                </b-form-tags>
                                
                            </b-form-group>

                            <b-form-group
                                label="LogLevels"
                                label-for="filter-log-levels"
                                label-cols-sm="3"
                                label-align-sm="right"
                                label-size="sm"
                                label-class="font-weight-bold pt-0"
                                description="Select the log level(s) that should be included."
                                class="mb-0"
                            >

                                <b-form-select
                                    id="filter-log-levels"
                                    v-model="logLevels"
                                    :options="optionsForLogLevels"
                                    multiple
                                    :select-size="4"></b-form-select>
                            </b-form-group>
                            
                            <b-form-group>
                                <b-form-group
                                    label="Start Time (UTC)"
                                    label-for="filter-start-time"
                                    label-cols-sm="3"
                                    label-align-sm="right"
                                    label-size="sm"
                                    label-class="font-weight-bold pt-0"
                                    description="Select the start time (UTC) of the bucket."
                                    class="mb-0"
                                >
                                    <b-form-timepicker
                                        id="filter-time"
                                        v-model="startTime"
                                        :hour12="false"
                                        reset-button
                                        now-button
                                        :state="isStartTimeValid"
                                        @input="onStartTimeUpdated"
                                        locale="en"
                                        hourCycle="h23"
                                    >
                                    </b-form-timepicker>
                                    
                                </b-form-group>

                                <b-form-group
                                    label="End Time (UTC)"
                                    label-for="filter-end-time"
                                    label-cols-sm="3"
                                    label-align-sm="right"
                                    label-size="sm"
                                    label-class="font-weight-bold pt-0"
                                    description="Select the end time (UTC) of the bucket."
                                    class="mb-0"
                                >
                                    <b-form-timepicker
                                        id="filter-end-time"
                                        v-model="endTime"
                                        :hour12="false"
                                        reset-button
                                        now-button
                                        :state="isEndTimeValid"
                                        @input="onEndTimeUpdated"
                                        locale="en"
                                        hourCycle="h23"
                                    >
                                    </b-form-timepicker>
                                </b-form-group>
                                
                            </b-form-group>
                            
                            <b-form-group
                                label="Limit"
                                label-for="filter-limit"
                                label-cols-sm="3"
                                label-align-sm="right"
                                label-size="sm"
                                label-class="font-weight-bold pt-0"
                                description="Select the limit in steps of 100."
                                class="mb-0"
                            >
                                <b-form-spinbutton
                                    id="filter-limit"
                                    v-model="limit"
                                    min="100"
                                    max="500"
                                    step="100"
                                    inline
                                ></b-form-spinbutton>
                            </b-form-group>
                            
                        </b-form-group>
                    </b-card>
                        
                    </b-collapse>
                </b-col>
                    
            </b-row>
            
            <b-row>
                <b-col>
                    <hr>
                </b-col>
            </b-row>
            
            <b-row>
                <b-col>
                    
                    <b-table :items="getItems"
                             :fields="fields"
                             responsive
                             fixed
                             striped
                             borderless
                             head-variant="light"
                             :filter="textSearchFilter"
                             :filter-included-fields="filterOn"
                             hover
                             show-empty
                             small
                             selectable
                             select-mode="single"
                             ref="selectableTable"
                             @row-selected="onLogsSelected">
                        
                        <template v-slot:table-colgroup>
                            <col style="width: 11rem" />
                            <col style="width: 6rem" />
                            <col style="width: 7rem" />
                            <col style="width: 16rem" />
                            <col style="width: auto">
                        </template>
                        
                        <template #cell(logLevel)="data">
                            <b-badge :variant="logLevelColor(data.value)">
                                <font-awesome-icon :icon="['fas', 'heartbeat']" v-show="data.value === 'Critical'"></font-awesome-icon>
                                <font-awesome-icon icon="times-circle" v-show="data.value === 'Error'"></font-awesome-icon>
                                <font-awesome-icon icon="exclamation-triangle" v-show="data.value === 'Warning'"></font-awesome-icon>
                                <font-awesome-icon icon="exclamation-circle" v-show="data.value === 'Information'"></font-awesome-icon>
                                <font-awesome-icon :icon="['far', 'life-ring']" v-show="data.value === 'Debug'"></font-awesome-icon>
                                <font-awesome-icon icon="info-circle" v-show="data.value === 'Trace'"></font-awesome-icon>
                                {{ data.value }}
                            </b-badge>
                        </template>
                        
                        <template #row-details="row">
                            <b-card>

                                <b-row class="mb-2">
                                    <b-col>
                                        <pre>{{ JSON.stringify(asJson(row.item), null, 2) }}</pre>
                                    </b-col>
                                </b-row>

                                <b-button-group>
                                    <b-button 
                                        size="sm" 
                                        @click="clearSelectedLog(row)"
                                        v-b-tooltip.hover title="Hide this expanded view."
                                    >
                                        Close
                                    </b-button>
                                    <b-button 
                                        size="sm" 
                                        @click="copyTextToClipboard(JSON.stringify(asJson(row.item), null, 2))"
                                        v-b-tooltip.hover title="Copy the content as JSON to the clipboard."
                                    >
                                        <font-awesome-icon icon="clipboard"></font-awesome-icon>
                                        Copy to clipboard
                                    </b-button>
                                </b-button-group>
                                
                            </b-card>
                        </template>
                        
                    </b-table>
                    
                </b-col>
            </b-row>            
        </b-container>
    </div>
</template>

<script>
    import axios from "axios";
    import {createNamespacedHelpers} from "vuex";
    import Log from './Log';

    const {mapActions, mapGetters} = createNamespacedHelpers('configuration');

    const defaults = {
        Limit: 100,
        StartTime: null,
        EndTime: null,
        AllLogLevels: ['Trace','Debug','Information','Warning','Error','Critical'],
        TextSearchFilter: null,
        ProcessIds: []
    };
    
    export default {
        name: "LiveLogView",
        data: () => ({
            filteredLogs: [],
            refreshEnabled: true,
            isLoading: false,
            textSearchFilter: defaults.TextSearchFilter,
            showFilters: false,
            appliedFilters: [],
            processIds: defaults.ProcessIds,
            startTime: defaults.StartTime,
            endTime: defaults.EndTime,
            isStartTimeValid: true,
            isEndTimeValid: true,
            sortDescending: false,
            limit: defaults.Limit,
            filterOn: ['message'],
            fields: [
                { key: 'serviceName' },
                { key: 'processId'  },
                { key: 'logLevel'  },
                { key: 'timestamp' },
                { key: 'message' }
            ],
            serviceNames: [],
            optionsForServiceNames: [],
            logLevels: defaults.AllLogLevels,
            optionsForLogLevels: defaults.AllLogLevels.map(x => ({value: x, text: x})),
        }),
        watch: {
            serviceNames() {
                this.syncFilters();
            },
            logLevels() {
                this.syncFilters();
            },
            processIds() {
                this.syncFilters();
            },
            limit() {
                this.syncFilters();
            }
        },
        async mounted() {
            await this.getExistingServices();
            this.serviceNames = this.existingServices;
            this.optionsForServiceNames = this.existingServices.map(x => ({value: x, text: x}));
            
            await this.reload();
            setInterval(async function() {
               await this.reload();
            }.bind(this), 1000);
            document.getElementById('vue-loader').remove();
        },
        computed: {
            /**
             * Map the getters from Vuex.
             */
            ...mapGetters(['existingServices']),
            getItems() {                
                if (this.sortDescending)
                    return this.filteredLogs.slice().reverse();
                
                return this.filteredLogs;
            },
            logLevelValues() {
                return this.logLevels.map( (level) => {
                    switch (level) {
                        case 'Trace': return 0;
                        case 'Debug': return 1;
                        case 'Information': return 2;
                        case 'Warning': return 3;
                        case 'Error': return 4;
                        case 'Critical': return 5;
                        default: return 0;
                    }
                });
            },
            serviceNameValues() {
                return this.serviceNames.map( (name) => {
                    return this.existingServices.indexOf(name);
                });
            },
            showAppliedFilters: {
                get() {
                    return this.textSearchFilter !== defaults.TextSearchFilter ||
                        this.logLevels.length !== defaults.AllLogLevels.length ||
                        this.serviceNames.length !== this.existingServices.length ||
                        this.processIds.length !== 0 ||
                        this.startTime !== defaults.StartTime ||
                        this.endTime !== defaults.EndTime ||
                        this.limit !== defaults.Limit;
                },
                set(_newValue) {
                  // don't do anything here, just needed for the v-model. Using v-model make sure to operate without delay. 
                }
            }
        },
        methods: {
            /**
             * Map the actions from Vuex.
             */
            ...mapActions(['getExistingServices']),
            copyTextToClipboard(text) {                
                navigator.clipboard.writeText(text).then(() => {
                    this.showSuccessToast('Copying to clipboard was successful!');
                }).catch(error => {
                    this.showErrorToast(`Failed to copy to clipboard. Reason: ${error.response.data.error}`);
                });
            },
            onClearAllAppliedFilters() {
                this.textSearchFilter = defaults.TextSearchFilter;
                this.logLevels = defaults.AllLogLevels;
                this.serviceNames = this.existingServices;
                this.processIds = defaults.ProcessIds;
                this.startTime = defaults.StartTime;
                this.endTime = defaults.EndTime;
                this.limit = defaults.Limit;
                this.syncFilters();
            },
            onRemoveTag({tag, removeTag}) {
                if (this.logLevels.includes(tag)) {
                    this.logLevels = this.arrayRemove(this.logLevels, tag);
                    removeTag(tag);
                    if (this.logLevels.length === 0) {
                        this.logLevels = defaults.AllLogLevels;
                    }
                } else if (this.serviceNames.includes(tag)) {
                    this.serviceNames = this.arrayRemove(this.serviceNames, tag);
                    removeTag(tag);
                    if (this.serviceNames.length === 0) {
                        this.serviceNames = this.existingServices;
                    }
                } else if (this.textSearchFilter === tag) {
                    this.textSearchFilter = defaults.TextSearchFilter;
                    removeTag(tag);
                } else if (`${this.limit}` === tag) {
                    this.limit = defaults.Limit;
                    removeTag(tag);
                } else if (this.processIds.includes(tag)) {
                    this.processIds = this.arrayRemove(this.processIds, tag);
                    removeTag(tag);
                } else if (`start:${this.startTime}` === tag) {
                    this.startTime = defaults.StartTime;
                    removeTag(tag);
                } else if (`end:${this.endTime}` === tag) {
                    this.endTime = defaults.EndTime;
                    removeTag(tag);
                }
            },
            arrayRemove(array, value) {
              return array.filter(function(element){
                 return element !== value; 
              });
            },
            syncFilters() {
                this.appliedFilters = [];

                this.logLevels.forEach(logLevel => {
                    this.appliedFilters.push(logLevel);
                });

                this.serviceNames.forEach(serviceName => {
                    this.appliedFilters.push(serviceName);
                });
                
                if (this.textSearchFilter) {
                    this.appliedFilters.push(this.textSearchFilter);
                }
                
                this.processIds.forEach(processId => {
                   this.appliedFilters.push(processId); 
                });
                
                if (this.startTime) {
                    this.appliedFilters.push(`start:${this.startTime}`);
                } else {
                    this.arrayRemove(this.appliedFilters, `start:${this.startTime}`);
                }
                
                if (this.endTime) {
                    this.appliedFilters.push(`end:${this.endTime}`);
                } else {
                    this.arrayRemove(this.appliedFilters, `end:${this.endTime}`);
                }
                
                if (this.limit !== 100) {
                    this.appliedFilters.push(this.limit);
                }
            },
            onLogsSelected(logs) {
                logs.forEach(log => {
                   log._showDetails = true;
                });
                
                this.refreshEnabled = false;
            },
            clearSelectedLog(row) {
                row.item._showDetails = false;
                this.$refs.selectableTable.clearSelected();
            },
            logLevelColor(logLevel) {
                switch (logLevel) {
                    case 'Critical':
                    case 'Error':
                        return 'danger';
                        
                    case 'Warning':
                        return 'warning';
                        
                    case 'Information':
                        return 'primary';
                    
                    case 'Debug':
                        return 'info';
                    
                    default:
                        return 'light';
                }
            },
            asJson(log) {
                return {
                    ServiceName: log.serviceName,
                    ProcessId: log.processId,
                    LogLevel: log.logLevel,
                    Timestamp: log.timestamp, 
                    Message: log.message
                };
            },
            onStartTimeUpdated(time) {
                this.isStartTimeValid = this.validateTime(time);                
                this.syncFilters();
            },
            onEndTimeUpdated(time) {
                this.isEndTimeValid = this.validateTime(time);
                this.syncFilters();
            },
            validateTime(time) {
                return /^([0-1]?\d|2[0-4]):([0-5]\d)(:[0-5]\d)?$/.test(time);
            },
            async reload() {
                if (!this.refreshEnabled || this.isLoading)
                    return;

                this.isLoading = true;
                
                try {
                    const request = {
                        logLevels: this.logLevelValues,
                        serviceNames: this.serviceNameValues,
                        processIds: this.processIds,
                        searchInMessage: this.textSearchFilter,
                        startBucket: this.isStartTimeValid ? this.startTime : null,
                        endBucket: this.isEndTimeValid ? this.endTime : null,
                        limit: this.limit
                    };
                    const response = await axios.post('/api/v1/live-logs/', request);
                    this.filteredLogs = [];
                    response.data.forEach(log => {
                        let localLog = new Log(log, this.existingServices);
                        this.filteredLogs.push(localLog);
                    });
                    this.syncFilters();
                } catch (e) {
                    console.error(e);
                    this.showErrorToast('Failed to fetch logs.');
                } finally {
                    this.isLoading = false;
                }
            },            
            async togglePause() {
                this.refreshEnabled = !this.refreshEnabled;
            }
        }
    }
</script>

<style scoped>
    .badge {
        width: 6rem;
    }
</style>