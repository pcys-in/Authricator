# Authricator
**Authricator** is *simple, powerful and highly configurable* **Authentication and Authorization Framework** for dotnet core which uses *JWT* and supports for enforcing authorization like *UBAC, RBAC and ABAC Access Controls*.

It is designed to be *Lightweight, Efficient and Human-Readable Access Controls.*

# Why?
There are very limited Auth. Frameworks which are **lightweight,  pluggable, highly configurable** and which supports **ABAC** (Attribute-Based Access Control). Many Existing Frameworks are feature rich but heavy, to address these limitations, this project was created.

This Project aims be a slim, pluggable module which helps in rapid integration into new and existing projects. This is made open-source so that community can benefit from it, enhance it.


# How it Works
Authricator is inspired from IAM policies and permissions and uses the same at its core to define Access Controls. It is **Priority-based**, by which underlying Policies/Permissions can be **overridden**.

Authricator uses Subject(s), which allows the principal entity to be a User or Device or an Object (Basically Anything which you want to have access-control).

It Supports
1) **UBAC** *(User/Subject-Based Access Control)* - Permission directly assigned to the Subject.
2) **RBAC** *(Role-Based Access Control)* - Policies with permissions can be created and assigned to an Subject.
3) **ABAC** *(Attribute-Based Access Control)* - Attributes of a Subject can be used to dynamically process the Authorization Result in runtime.

## Highly Configurable
Authricator uses *JSON* under the hood for its configurations. This allows it to be File based or DB based and allows Custom *ConfigurationProviders*.

## What and Why ABAC?

**ABAC** (Attribute-Based Access Control) is one of the *highlights* of this project, it allows dynamic results which are computed runtime. It uses a *JavaScript Interpreter / Engine*  under the hood, to process the Attributes, and return the Authorization Result.

Using a *[JavaScript Engine](https://github.com/sebastienros/jint)*, has end-less possibilities as it allows to *write Authorization / Access-Control Polices as **code***. This makes it the most,  highly-configurable solution.

There are many times where the Access-Control policy is so complex, that the result can only be achieved using an ABAC. It is user-intuitive, easy to configure, and anyone with authority can modify it without the need to change the code-base.

For Example : Permission to business-critical data, may sometime require Complex Access-Control Policy which *cannot* be managed with a *UBAC, RBAC models*. ABAC allows dynamic attributes like Department, Groups, Position, IP, Location, Date/Time, Status, etc. and any other attributes associated with a subject to affect the result of an authorization request.  

## License

This project is licensed under the  [*GNU General Public License v3.0 license*](https://github.com/pcys-in/Authricator/blob/main/LICENSE) and maintained by Patronum Cyber Solutions.
