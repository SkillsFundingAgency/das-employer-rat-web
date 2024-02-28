## ⛔Never push sensitive information such as client id's, secrets or keys into repositories including in the README file⛔

# Employer Request Apprentice Training Web

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_apis/build/status/das-employer-aan-web?branchName=main)](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_build/latest?definitionId=3244&branchName=main)
[![Quality gate](https://sonarcloud.io/api/project_badges/quality_gate?project=SkillsFundingAgency_das-employer-aan-web)](https://sonarcloud.io/summary/new_code?id=SkillsFundingAgency_das-employer-aan-web)
[![Confluence Page](https://img.shields.io/badge/Confluence-Project-blue)](https://skillsfundingagency.atlassian.net/wiki/spaces/NDL/pages/3867705345/AAN+Employer+Solution+Architecture)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)

This web solution is part of Request Apprentice Training (RAT) project. Here the employer users can search for apprentice training using the extended search facility and optionally create requests for training course.

## How It Works
Users are expected to register themselves in the Employer portal. Once registered they will have access to extended search facilty. 
When running this locally, with stub sign-in enabled, the launch url should be `https://localhost:7701/`

## 🚀 Installation

### Pre-Requisites
* A clone of this repository
* Optionally an Azure Active Directory account with the appropriate roles.
* The Outer API [das-apim-endpoints](https://github.com/SkillsFundingAgency/das-apim-endpoints/tree/master/src/EmployerRequestApprenticeTraining) should be available either running locally or accessible in an Azure tenancy.

### Config
You can find the latest config file in [das-employer-config repository](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-employer-request-apprentice-training/SFA.DAS.EmployerRequestApprenticeTraining.Web.json)

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
  "ResourceEnvironmentName": "LOCAL",
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
