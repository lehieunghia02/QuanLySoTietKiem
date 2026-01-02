# Savings Account Management System (ASP.NET MVC)

A **Dockerized ASP.NET MVC web application** for managing personal savings plans, designed with real-world deployment practices and cloud readiness in mind.  
The system helps users create, track, and optimize their savings goals, with secure configuration management and scalable architecture.

---

## 🚀 Key Features

### 👤 User Management
- User registration and authentication
- Profile management with avatar upload (AWS S3)
- Secure session handling

### 💰 Savings Plan Management
- Create savings plans with:
    - Target amount
    - Saving duration (months)
    - Monthly or one-time contributions
- Track progress with percentage-based indicators
- Highlight recommended saving durations (e.g. 12 months – most popular)

### 📊 Dashboard & Visualization
- Clean and responsive dashboard UI
- Real-time progress tracking for savings goals
- Clear separation of business logic and presentation

### 📄 Export & Reporting
- Export savings plans to **PDF** and **Excel**
- Designed for real-world financial reporting use cases

### ☁️ Cloud & Infrastructure
- Fully Dockerized application
- Externalized configuration (`appsettings.json`)
- Deployed on **AWS EC2 Free Tier**
- Reverse proxy with **Nginx**
- HTTPS enabled via **Let’s Encrypt SSL**

---

## 🧠 AI (Planned / Extensible)
- AI-powered financial advisor for personalized saving recommendations
- Savings goal feasibility analysis based on user data
- Designed to integrate LLM-based reasoning services

---

## 🛠 Tech Stack

**Backend**
- ASP.NET MVC (.NET)
- Entity Framework Core
- SQL-based relational database

**Frontend**
- Razor Views
- Bootstrap (responsive UI)

**Infrastructure**
- Docker
- AWS EC2 (t3.micro – Free Tier)
- Nginx (Reverse Proxy)
- Let’s Encrypt (SSL)
- AWS S3 (Avatar storage)
