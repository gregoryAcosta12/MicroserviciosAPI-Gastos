# 🏦 API-Gastos-Microservicios

<div align="center">

![.NET](https://img.shields.io/badge/.NET-8.0-blue?style=for-the-badge&logo=dotnet)
![Status](https://img.shields.io/badge/Status-En%20Desarrollo-yellow?style=for-the-badge)
![Microservices](https://img.shields.io/badge/Microservices-4-green?style=for-the-badge)
![Docker](https://img.shields.io/badge/Docker-Ready-blue?style=for-the-badge&logo=docker)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-Enabled-orange?style=for-the-badge&logo=rabbitmq)
![Redis](https://img.shields.io/badge/Redis-Caching-red?style=for-the-badge&logo=redis)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)

**Sistema de Gestión de Gastos basado en Arquitectura de Microservicios**

</div>

---

## 📋 Tabla de Contenidos

- [Estado del Proyecto](#estado-del-proyecto)
- [Descripción General](#descripción-general)
- [Arquitectura](#arquitectura)
- [Tecnologías Utilizadas](#tecnologías-utilizadas)
- [Microservicios](#microservicios)
- [Comunicación entre Servicios](#comunicación-entre-servicios)
- [Requisitos Previos](#requisitos-previos)
- [Instalación y Ejecución](#instalación-y-ejecución)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [API Endpoints](#api-endpoints)
- [Base de Datos](#base-de-datos)
- [Pruebas](#pruebas)
- [Despliegue](#despliegue)
- [Roadmap](#roadmap)
- [Contribución](#contribución)
- [Licencia](#licencia)

---

## 🚧 Estado del Proyecto

<div align="center">

| Módulo | Estado | Observaciones |
|--------|--------|---------------|
| **API-Gateway** | 🟡 En Desarrollo | Implementación base completada |
| **Gasto-Service** | 🟡 En Desarrollo | CRUD básico funcionando |
| **Categoria-Service** | 🟡 En Desarrollo | CRUD básico funcionando |
| **Usuario-Service** | 🟡 En Desarrollo | Autenticación JWT implementada |
| **Shared-Kernel** | 🟢 Estable | Biblioteca base completada |
| **RabbitMQ** | 🟡 En Desarrollo | Comunicación básica implementada |
| **Redis** | 🟡 En Desarrollo | Caching configurado |
| **Tests** | 🔴 Pendiente | Pruebas en fase inicial |

</div>

> **⚠️ NOTA:** Este proyecto se encuentra en **fase activa de desarrollo**. Algunas funcionalidades pueden estar incompletas o en proceso de implementación. Las contribuciones y sugerencias son bienvenidas.

---

## 📖 Descripción General

**API-Gastos-Microservicios** es un sistema de gestión de gastos personales construido con **arquitectura de microservicios** utilizando **.NET 8**. El sistema permite a los usuarios:

- ✅ Registrar y autenticarse
- ✅ Crear, editar y eliminar gastos
- ✅ Organizar gastos por categorías
- ✅ Generar reportes y estadísticas
- ✅ Comunicación asíncrona entre servicios via RabbitMQ
- ✅ Caching distribuido con Redis

---

## 🏗️ Arquitectura
