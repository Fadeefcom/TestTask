FormSubmissions

Backend: ASP.NET Core DDD microservice with dynamic form definitions and submissions stored in memory.
Frontend: Vue 3 + Vite single-page app with one sample form and submissions list.

Test Task 2

We can use cloud object storage such as Azure Blob Storage or AWS S3 for the binary files, and store only metadata in a database. For metadata we can choose either a wide-column / NoSQL database (good for flexible schema, large volume, and distributed scaling) or a traditional relational database like PostgreSQL. If we expect heavy download traffic, it is useful to place a CDN in front of the storage and route users to the nearest edge/data center.

FormSubmissions API should never stream or store large files itself. It only manages attachment metadata and issues short-lived upload/download URLs (pre-signed/SAS), so the UI uploads and downloads directly to/from object storage.

Optionally, a background processor can subscribe to “file uploaded” events to validate file integrity, run virus scanning, generate previews, and update attachment status.
