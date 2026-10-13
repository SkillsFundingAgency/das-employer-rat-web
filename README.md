## ⛔Never push sensitive information such as client id's, secrets or keys into repositories including in the README file⛔

# Employer Request Apprentice Training Web

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_apis/build/status/das-employer-rat-web?branchName=main)](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_build?definitionId=3645)
[![Quality gate](https://sonarcloud.io/api/project_badges/quality_gate?project=SkillsFundingAgency_das-employer-rat-web)](https://sonarcloud.io/summary/new_code?id=SkillsFundingAgency_das-employer-rat-web)
[![Confluence Page](https://img.shields.io/badge/Confluence-Project-blue)](https://skillsfundingagency.atlassian.net/wiki/spaces/NDL/pages/4708204548/Request+Apprenticeship+Training+Employer+Provider)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)

This web solution is part of Request Apprentice Training (RAT) project. Here the employer users can search for apprentice training using the extended search facility and optionally create requests for training course.

## How It Works
Users are expected to register themselves in the Employer portal. Once registered they will have access to extended search facilty. 
When running this locally, with stub sign-in enabled, the launch url should be `https://localhost:7701/`

## 🚀 Installation

### Pre-Requisites
* A clone of this repository
* A code editor that supports.NET 10.0 e.g. Visual Studio 2026
* Optionally an Azure Active Directory account with the appropriate roles.
* The Outer API [das-apim-endpoints](https://github.com/SkillsFundingAgency/das-apim-endpoints/tree/master/src/EmployerRequestApprenticeTraining) should be available either running locally or accessible in an Azure tenancy.
* Azure Table Storage for config (Azurite and Azure Storage Explorer can be used locally)

### Config

You can find the latest config file in [das-employer-config repository](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-employer-rat-web/SFA.DAS.EmployerRequestApprenticeTraining.Web.json)

Add an entry to Azure Table Storage config

1. Start Azurite and open it in Azure Storage Explorer
2. Create a table called Configuration (if it does not already exist)
3. Add a new entry with the following properties

* PartitionKey : LOCAL
* RowKey : SFA.DAS.EmployerRequestApprenticeTraining.Web_1.0
* Data : the JSON for this service from the das-employer-config repository


In the web project, if not exist already, add `AppSettings.Development.json` file with following content:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConfigurationStorageConnectionString": "UseDevelopmentStorage=true;",
  "SFA.DAS.EmployerRequestApprenticeTraining.Web,SFA.DAS.Employer.Shared.UI,SFA.DAS.Encoding:EncodingConfig,SFA.DAS.Employer.GovSignIn",
  "EnvironmentName": "LOCAL",
  "ResourceEnvironmentName": "LOCAL",
  "cdn": {
    "url": "https://das-test-frnt-end.azureedge.net"
  },
  "StubEmail": "someemail",
  "StubId": "someid",
  "StubAuth": true
} 
```

## Technologies
* .NetCore 8.0
* NUnit
* Moq
* FluentAssertions
* RestEase
* MediatR
