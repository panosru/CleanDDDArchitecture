# Security Policy

## Reporting a vulnerability

Report vulnerabilities privately through GitHub: open the repository's **Security** tab and choose **Report a vulnerability**. Please do not open a public issue.

Include what is affected, the impact, and steps to reproduce. You will get an acknowledgement within a few days, and credit in the advisory once a fix is released, unless you prefer otherwise.

Vulnerabilities in the Aviant library itself (`Library/Aviant`) can be reported at [tecfinity/Aviant](https://github.com/tecfinity/Aviant/security/advisories/new).

## Scope and defaults

This is a reference application. The development settings in `docker/compose.yaml` and the Aspire AppHost, such as the JWT keys and database passwords, are public dev values and must never be used in a deployed environment. Supply real secrets through your platform's secret store.

Dependencies are audited on every build: a package with a known moderate, high or critical advisory, including transitive ones, fails CI. Dependabot proposes updates weekly.
