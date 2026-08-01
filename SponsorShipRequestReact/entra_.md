# [Microsoft Entra admin center](https://entra.microsoft.com/)

When you are inside the Azure Portal or Entra Admin Center creating a new tenant, you see options like `Microsoft Entra ID` 
and `Azure AD B2C`. The naming convention here is a bit of a historical overlap because Microsoft is currently transitioning between generations of their product.

If you are developing a **brand-new public-facing application**, you should look for the setup option called 
**"External" tenant** (which runs **Microsoft Entra External ID**).

Here is how the options on your screen translate to your exact goal:

### The Modern Way to Build a Public-Facing App
When creating a tenant for your consumer application, the setup wizard will usually ask you to select a **Tenant type**.                       ┌──────────────────────────┐
```                    
                       ┌──────────────────────────┐   
                       │  What are you building?  │
                       └─────────────┬────────────┘
                                     │
            ┌────────────────────────┴────────────────────────┐
            ▼                                                 ▼
   [ Workforce Tenant ]                              [ External Tenant ]
   - Select: Microsoft Entra ID                      - Select: External (Entra External ID)
   - For: Your internal developers,                  - For: Your public-facing app
     employees, and enterprise infra.                  customers (The new B2C).

```
- **Do NOT choose "Azure AD B2C"**: As you noted, this is the legacy platform. It is being phased out (support ends in 2030, and Microsoft stopped selling it to new customers).  
- **Do NOT choose standard "Microsoft Entra ID" for your customers**: If you select the standard option, you are creating a "Workforce" tenant. This expects users to have organizational emails (like name@yourcompany.com) and charges you standard enterprise seat licensing.
- **DO choose "External" (Microsoft Entra External ID)**: This is Microsoft's active, modern platform explicitly built for public-facing applications.

## App Registrations & Enterprise Applications
An easy analogy clears this up instantly: Think of App Registrations as the blueprints or source code for a software product, and Enterprise Applications as the actual instances running on a server that you can monitor, secure, and assign to users.

### 1. App Registrations (The Developer View)
This menu is where an application's identity is born. If you are building a custom Web API, a mobile app, or a daemon service, you start here.  

- **What it represents**: The global definition (the "Application Object") of your software. 
- **What you do here:**
  - Define authentication rules (e.g., Single-Tenant vs. Multi-Tenant).  
  - Generate secure credentials (Client Secrets or Certificates).  
  - Define Redirect URIs (where tokens should be sent after a login).
  - Declare API permissions (like requesting access to Microsoft Graph or your own secure backend APIs).
- **Scope:** It lives natively in the home tenant where the application was originally coded and registered

### 2. Enterprise Applications (The IT Admin View)
This menu is where you manage and govern how an application behaves within a specific organization's tenant.  
- **What it represents**: The local instance (the Service Principal) of an application.  
- **What you do here**:
  - **Assign Users & Groups**: Dictate exactly who in the company is allowed to click on and log into that app.  
  - **Enforce Security Policies**: Hook up Conditional Access Policies (e.g., requiring Multi-Factor Authentication (MFA) or blocking logins if the user is outside the country).  
  - **Integrate Third-Party SaaS**: If your company buys a subscription to Salesforce, Zoom, or Slack, you add them straight from the Entra Gallery into Enterprise Applications to configure corporate Single Sign-On (SSO).
  - **Audit Logging**: Monitor sign-in logs to see exactly who is utilizing the application and detect anomalous behavior.

They share the exact same Application (Client) ID, but they serve entirely distinct roles:

| Feature / Property | App Registrations | Enterprise Applications |
| :---------------- | :------: | :---- |
| Primary Persona | Software Developers / Architects | IT Administrators / Security Ops |
| Underlying Object | Application Object (The Blueprint) | Service Principal (The Concrete Instance) |
| Custom App Development | Yes, this is where you input code configurations. | No, you only configure access policies here. |
| Third-Party SaaS (e.g., Salesforce) | No, you don't own the code blueprint. | Yes, you manage your corporate access here. |
| Multi-Tenant Behavior | There is only one registration globally (in the developer's home tenant). | A copy of this object is created in every customer tenant that consents to use the app. |


If you are **building your own software**, you always start in **App Registrations** to create the identity blueprint. The moment you do that, Entra ID automatically generates the corresponding instance in **Enterprise Applications** behind the scenes so it can be managed within your directory.

Practical experinece:
1. when creating an new app registration, it will also create a corresponding enterprise application in the same tenant.
    - eg: sponsorship_r.
1. when creating a new enterprise application for custom software, it will also create a corresponding app registration in the same tenant.
    - eg: sponsorship.


### prompt: 
When to use Certificates and secrets? I have register a single teneant web app in the Single Sign on page it show OIDC sign on and no option to download .cer file. Give me some idea when to use Manage certificate, showing in this tab.

[Manage cerificates](https://learn.microsoft.com/en-us/entra/identity/enterprise-apps/tutorial-manage-certificates-for-federated-single-sign-on)

### Response:
The reason you aren't seeing an option to download a `.cer `file in the Single Sign-On (SSO) section is because your app is configured for OpenID Connect (OIDC).

Downloadable certificates are typically used for older **SAML 2.0** identity federation. With OIDC, token validation happens automatically using a public JSON Web Key Set (JWKS) endpoint hosted by Microsoft, so your application code downloads the public keys dynamically at runtime—no manual file management required.

However, the **"Certificates & Secrets"** menu in your App Registration serves a completely different purpose. It isn't used for user logins; it is used **when your web app needs to authenticate itself to Microsoft Entra ID** (app-to-app or app-to-API communication).

### Client Secrets vs. Certificates: The Core Difference

When your backend code needs to prove its identity to Entra ID (for instance, to get an access token via the Client Credentials flow to read Microsoft Graph data or connect to Azure Key Vault), it must provide a credential.
### 1. Client Secrets (The Password Approach)

A secret is essentially a long, auto-generated password string.
 - When to use it: Great for local development, quick prototyping, or non-critical secondary applications.
 - The Risk: It is a string of text. If a developer accidentally commits it to a GitHub repository, logs it, or stores it in plain text in an appsettings.json file, the app is compromised. They also have a maximum expiration of 2 years, meaning you have to manually rotate them before they break production code.

2. Certificates (The Cryptographic Approach)

Instead of a password, you upload the public key (.cer) of an asymmetric key pair to Entra ID, and your application retains the private key (.pfx).
 - **When to use it**: Mandatory for production enterprise applications. * The Benefit: High security. Your code uses the private key to signs a temporary client assertion token and sends that assertion to Entra ID. The actual private key never travels across the network, and even if an attacker intercepts the network traffic or gains read-only access to your App Registration portal, they cannot steal the credential because the public key (.cer) is useless without the private key.

### What Should You Use in Practice?

Since you are architecting a modern application stack, you actually want to avoid managing either of these manually if you can. Here is the modern hierarchy of choices for backend authentication:

### Tier 1: Managed Identities (Best Practice)

If your Web API or web application is hosted inside Azure (like an Azure App Service, Azure Functions, or AKS), you should completely bypass the "Certificates & Secrets" tab.

Turn on Managed Identity for your Azure resource. In your .NET code, you simply use the `Azure.Identity SDK`:
```
C#

var credential = new DefaultAzureCredential();
```

Entra ID handles the credential rotation automatically every few hours behind the scenes. You have zero connection strings, zero secrets, and zero certificates to manage or leak.

### Tier 2: Certificates + Azure Key Vault (If hosted outside Azure)

If you are hosting your app outside of Azure (e.g., on-premises IIS, AWS, or GCP) but still need production-grade enterprise security:

  - Generate a self-signed or CA-signed certificate.
  - Upload the .cer file to the Certificates & Secrets tab in Entra ID.
  - Securely store the private key .pfx in a hardware security module or local secure environment variable, allowing your backend code to pull it securely to sign authentication requests.

### Tier 3: Client Secrets (Local Dev Only)



Use secrets strictly for your local localhost development machine environment so you can get up and running quickly without dealing with local certificate stores. Just make sure they are never committed to your source control.


# [Microsoft identity platform](https://learn.microsoft.com/en-us/entra/identity-platform/)
[Samples](https://learn.microsoft.com/en-us/entra/identity-platform/sample-v2-code?tabs=apptype)