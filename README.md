TBCPAY.GE

Overview

TBCPAY.GE is a payment processing application built with .NET Core, designed to handle user authentication, multi-factor authentication (MFA) via SMS, and payment initiation using Stripe. 


The application supports Google OAuth for user login and provides a secure API for managing transactions.


<img width="387" alt="image" src="https://github.com/user-attachments/assets/aa93e99f-890c-447f-b9df-0a4ec9a1cbc3" />



User Authentication: Register and login with email/password:

<img width="986" alt="A30583EE-BC33-4F91-BDF0-C4C546B8A1BB" src="https://github.com/user-attachments/assets/eb1d1418-c00b-4509-907f-86c0d47c0ee1" />
<img width="991" alt="9AAB07B3-097F-40EE-AC6E-D3F841CB85EE" src="https://github.com/user-attachments/assets/213bb26b-a54d-4371-a32f-fa5ee3fc03ad" />

or Google OAuth:

https://github.com/user-attachments/assets/90c0f329-042a-46e1-b3e8-c10cf62d2211

Stripe payment:


<img width="988" alt="image" src="https://github.com/user-attachments/assets/cc20a696-ffb5-431a-b924-623e6c77e5b7" />
<img width="1104" alt="image" src="https://github.com/user-attachments/assets/ef4d3800-1d6c-4e10-9a30-dff980656bb6" />



Multi-Factor Authentication (MFA): SMS-based MFA using Twilio for secure login. (currently not available, needs to fix)
Payment Processing: Initiate payments using Stripe.
Database: PostgreSQL for storing user and transaction data.
Dockerized Setup: Run the application using Docker and Docker Compose.
Project Structure
API: The main ASP.NET Core Web API project containing controllers and configuration.
Application: Business logic and command handlers (e.g., LoginUserCommandHandler).
Domain: Core entities and domain logic (e.g., User, Transaction).
Infrastructure: Data access layer with Entity Framework Core for PostgreSQL.

Prerequisites:
Docker and Docker Compose: To run the application in containers.
.NET SDK 9.0
PostgreSQL
Stripe Account
Google Cloud Console
Twilio account with active number




Current issue:



Twilio API Block Due to Exposed Secrets


<img width="744" alt="image" src="https://github.com/user-attachments/assets/eb78ac08-6fe4-430c-b628-9458dfa39cd7" />
<img width="981" alt="955DDA0F-1DE4-4E18-AADA-114845450861" src="https://github.com/user-attachments/assets/66e199a3-3d2e-4496-ac60-f66649e50e9f" />


It's credentials were accidentally exposed in the Git repository, leading Twilio to block my API for security reasons (I didn't knew about such consequence).
Testing authentication features (e.g., /api/identity/login, /api/identity/mfa/verify) that rely on Twilio for SMS-based MFA is currently unavailable.

I will fix it soon.

