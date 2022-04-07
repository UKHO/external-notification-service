{
  "lenses": {
    "0": {
    "order": 0,
    "parts": {
        "0": {
        "position": {
            "x": 0,
            "y": 0,
            "colSpan": 6,
            "rowSpan": 4
        },
        "metadata": {
            "inputs": [
            {
                "name": "options",
                "isOptional": true
            },
            {
                "name": "sharedTimeRange",
                "isOptional": true
            }
            ],
            "type": "Extension/HubsExtension/PartType/MonitorChartPart",
            "settings": {
            "content": {
                "options": {
                "chart": {
                    "metrics": [
                    {
                        "resourceMetadata": {
                        "id": "/subscriptions/${subscription_id}/resourceGroups/ens-${environment}-rg/providers/Microsoft.Web/sites/ens-${environment}-webapp"
                        },
                        "name": "HttpResponseTime",
                        "aggregationType": 4,
                        "namespace": "microsoft.web/sites",
                        "metricVisualization": {
                        "displayName": "Response Time",
                        "resourceDisplayName": "ens-${environment}-webapp"
                        }
                    }
                    ],
                    "title": "REQUEST LATENCY (Avg Response Time for ens-${environment}-webapp)",
                    "titleKind": 2,
                    "visualization": {
                    "chartType": 2,
                    "legendVisualization": {
                        "isVisible": true,
                        "position": 2,
                        "hideSubtitle": false
                    },
                    "axisVisualization": {
                        "x": {
                        "isVisible": true,
                        "axisType": 2
                        },
                        "y": {
                        "isVisible": true,
                        "axisType": 1
                        }
                    },
                    "disablePinning": true
                    }
                }
                }
            }
            }
        }
        },
        "1": {
        "position": {
            "x": 6,
            "y": 0,
            "colSpan": 6,
            "rowSpan": 4
        },
        "metadata": {
            "inputs": [
            {
                "name": "options",
                "isOptional": true
            },
            {
                "name": "sharedTimeRange",
                "isOptional": true
            }
            ],
            "type": "Extension/HubsExtension/PartType/MonitorChartPart",
            "settings": {
            "content": {
                "options": {
                "chart": {
                    "metrics": [
                    {
                        "resourceMetadata": {
                        "id": "/subscriptions/${subscription_id}/resourceGroups/ens-${environment}-rg/providers/Microsoft.Web/sites/ens-${environment}-webapp"
                        },
                        "name": "Http5xx",
                        "aggregationType": 7,
                        "namespace": "microsoft.web/sites",
                        "metricVisualization": {
                        "displayName": "Http Server Errors",
                        "resourceDisplayName": "ens-${environment}-webapp"
                        }
                    }
                    ],
                    "title": "ERROR RATE (Count Http Server Errors for ens-${environment}-webapp)",
                    "titleKind": 2,
                    "visualization": {
                    "chartType": 2,
                    "legendVisualization": {
                        "isVisible": true,
                        "position": 2,
                        "hideSubtitle": false
                    },
                    "axisVisualization": {
                        "x": {
                        "isVisible": true,
                        "axisType": 2
                        },
                        "y": {
                        "isVisible": true,
                        "axisType": 1
                        }
                    },
                    "disablePinning": true
                    }
                }
                }
            }
            }
        }
        },
        "2": {
        "position": {
            "x": 12,
            "y": 0,
            "colSpan": 6,
            "rowSpan": 4
        },
        "metadata": {
            "inputs": [
            {
                "name": "options",
                "isOptional": true
            },
            {
                "name": "sharedTimeRange",
                "isOptional": true
            }
            ],
            "type": "Extension/HubsExtension/PartType/MonitorChartPart",
            "settings": {
            "content": {
                "options": {
                "chart": {
                    "metrics": [
                    {
                        "resourceMetadata": {
                        "id": "/subscriptions/${subscription_id}/resourceGroups/ens-${environment}-rg/providers/Microsoft.Web/sites/ens-${environment}-webapp"
                        },
                        "name": "HealthCheckStatus",
                        "aggregationType": 1,
                        "namespace": "microsoft.web/sites",
                        "metricVisualization": {
                        "displayName": "Health check status",
                        "resourceDisplayName": "ens-${environment}-webapp"
                        }
                    }
                    ],
                    "title": "AVAILABILITY (Sum Data In for ens-${environment}-webapp)",
                    "titleKind": 2,
                    "visualization": {
                    "chartType": 2,
                    "legendVisualization": {
                        "isVisible": true,
                        "position": 2,
                        "hideSubtitle": false
                    },
                    "axisVisualization": {
                        "x": {
                        "isVisible": true,
                        "axisType": 2
                        },
                        "y": {
                        "isVisible": true,
                        "axisType": 1
                        }
                    },
                    "disablePinning": true
                    }
                }
                }
            }
            }
        }
        },
        "3": {
        "position": {
            "x": 0,
            "y": 4,
            "colSpan": 6,
            "rowSpan": 4
        },
        "metadata": {
            "inputs": [
            {
                "name": "options",
                "isOptional": true
            },
            {
                "name": "sharedTimeRange",
                "isOptional": true
            }
            ],
            "type": "Extension/HubsExtension/PartType/MonitorChartPart",
            "settings": {
            "content": {
                "options": {
                "chart": {
                    "metrics": [
                    {
                        "resourceMetadata": {
                        "id": "/subscriptions/${subscription_id}/resourceGroups/ens-${environment}-rg/providers/Microsoft.EventGrid/domains/ens-${environment}-eventgriddomain"
                        },
                        "name": "PublishSuccessCount",
                        "aggregationType": 1,
                        "namespace": "microsoft.eventgrid/domains",
                        "metricVisualization": {
                        "displayName": "Published Events",
                        "resourceDisplayName": "ens-${environment}-eventgriddomain"
                        }
                    },
                    {
                        "resourceMetadata": {
                        "id": "/subscriptions/${subscription_id}/resourceGroups/ens-${environment}-rg/providers/Microsoft.EventGrid/domains/ens-${environment}-eventgriddomain"
                        },
                        "name": "PublishFailCount",
                        "aggregationType": 1,
                        "namespace": "microsoft.eventgrid/domains",
                        "metricVisualization": {
                        "displayName": "Publish Failed Events",
                        "resourceDisplayName": "ens-${environment}-eventgriddomain"
                        }
                    }
                    ],
                    "title": "PUBLISHED EVENTS (Sum Published Events for ens-${environment}-eventgriddomain)",
                    "titleKind": 2,
                    "visualization": {
                    "chartType": 1,
                    "legendVisualization": {
                        "isVisible": true,
                        "position": 2,
                        "hideSubtitle": false
                    },
                    "axisVisualization": {
                        "x": {
                        "isVisible": true,
                        "axisType": 2
                        },
                        "y": {
                        "isVisible": true,
                        "axisType": 1
                        }
                    },
                    "disablePinning": true
                    }
                }
                }
            }
            }
        }
        },
        "4": {
        "position": {
            "x": 6,
            "y": 4,
            "colSpan": 6,
            "rowSpan": 4
        },
        "metadata": {
            "inputs": [
            {
                "name": "options",
                "isOptional": true
            },
            {
                "name": "sharedTimeRange",
                "isOptional": true
            }
            ],
            "type": "Extension/HubsExtension/PartType/MonitorChartPart",
            "settings": {
            "content": {
                "options": {
                "chart": {
                    "metrics": [
                    {
                        "resourceMetadata": {
                        "id": "/subscriptions/${subscription_id}/resourceGroups/ens-${environment}-rg/providers/Microsoft.EventGrid/domains/ens-${environment}-eventgriddomain"
                        },
                        "name": "DeliverySuccessCount",
                        "aggregationType": 1,
                        "namespace": "microsoft.eventgrid/domains",
                        "metricVisualization": {
                        "displayName": "Delivered Events",
                        "resourceDisplayName": "ens-${environment}-eventgriddomain"
                        }
                    },
                    {
                        "resourceMetadata": {
                        "id": "/subscriptions/${subscription_id}/resourceGroups/ens-${environment}-rg/providers/Microsoft.EventGrid/domains/ens-${environment}-eventgriddomain"
                        },
                        "name": "DeliveryAttemptFailCount",
                        "aggregationType": 1,
                        "namespace": "microsoft.eventgrid/domains",
                        "metricVisualization": {
                        "displayName": "Delivery Failed Events",
                        "resourceDisplayName": "ens-${environment}-eventgriddomain"
                        }
                    }
                    ],
                    "title": "DELIVERED EVENTS (Sum Delivered Events for ens-${environment}-eventgriddomain)",
                    "titleKind": 2,
                    "visualization": {
                    "chartType": 1,
                    "legendVisualization": {
                        "isVisible": true,
                        "position": 2,
                        "hideSubtitle": false
                    },
                    "axisVisualization": {
                        "x": {
                        "isVisible": true,
                        "axisType": 2
                        },
                        "y": {
                        "isVisible": true,
                        "axisType": 1
                        }
                    },
                    "disablePinning": true
                    }
                }
                }
            }
            }
        }
        },
        "5": {
        "position": {
            "x": 12,
            "y": 4,
            "colSpan": 6,
            "rowSpan": 4
        },
        "metadata": {
            "inputs": [
            {
                "name": "options",
                "isOptional": true
            },
            {
                "name": "sharedTimeRange",
                "isOptional": true
            }
            ],
            "type": "Extension/HubsExtension/PartType/MonitorChartPart",
            "settings": {
            "content": {
                "options": {
                "chart": {
                    "metrics": [
                    {
                        "resourceMetadata": {
                        "id": "/subscriptions/${subscription_id}/resourceGroups/ens-${environment}-rg/providers/Microsoft.EventGrid/domains/ens-${environment}-eventgriddomain"
                        },
                        "name": "DeadLetteredCount",
                        "aggregationType": 1,
                        "namespace": "microsoft.eventgrid/domains",
                        "metricVisualization": {
                        "displayName": "Dead Lettered Events",
                        "resourceDisplayName": "ens-${environment}-eventgriddomain"
                        }
                    }
                    ],
                    "title": "DEAD LETTERED EVENTS (Sum Dead Lettered Events for ens-${environment}-eventgriddomain)",
                    "titleKind": 2,
                    "visualization": {
                    "chartType": 1,
                    "legendVisualization": {
                        "isVisible": true,
                        "position": 2,
                        "hideSubtitle": false
                    },
                    "axisVisualization": {
                        "x": {
                        "isVisible": true,
                        "axisType": 2
                        },
                        "y": {
                        "isVisible": true,
                        "axisType": 1
                        }
                    },
                    "disablePinning": true
                    }
                }
                }
            }
            }
        }
        }
    }
    }
  },
  "metadata": {
    "model": {
    "timeRange": {
        "value": {
        "relative": {
            "duration": 24,
            "timeUnit": 1
        }
        },
        "type": "MsPortalFx.Composition.Configuration.ValueTypes.TimeRange"
    },
    "filterLocale": {
        "value": "en-us"
    },
    "filters": {
        "value": {
        "MsPortalFx_TimeRange": {
            "model": {
            "format": "local",
            "granularity": "auto",
            "relative": "7d"
            },
            "displayCache": {
            "name": "Local Time",
            "value": "Past 7 days"
            },
            "filteredPartIds": [
            "StartboardPart-MonitorChartPart-b556a964-ab02-4396-9dbb-d07eee5e609f",
            "StartboardPart-MonitorChartPart-b556a964-ab02-4396-9dbb-d07eee5e60f3",
            "StartboardPart-MonitorChartPart-b556a964-ab02-4396-9dbb-d07eee5e60ff",
            "StartboardPart-MonitorChartPart-b556a964-ab02-4396-9dbb-d07eee5e610c",
            "StartboardPart-MonitorChartPart-b556a964-ab02-4396-9dbb-d07eee5e61fe",
            "StartboardPart-MonitorChartPart-b556a964-ab02-4396-9dbb-d07eee5e620a"
            ]
        }
        }
    }
    }
  },
  "name": "ENS-${environment}-monitoring-dashboard",
  "type": "Microsoft.Portal/dashboards",
  "location": "INSERT LOCATION",
  "tags": {
    "hidden-title": "ENS-${environment}-monitoring-dashboard"
  },
  "apiVersion": "2015-08-01-preview"
}
