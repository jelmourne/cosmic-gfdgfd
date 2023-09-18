# Music Festival Event Management System
## Project Scope

Cosmic Queries is an application that is designed specifically to simplify the planning and management of a music festival. It serves as a database management system that features a user-friendly content management system (CMS) that helps organizers keep track and manage important aspects of their festival's components. Users will have the ability to easily handle important tasks such as scheduling, ticketing, production and various other aspects of the management process. The application would be able to connect to the festival's website where customers are able to find information about the festival as well as purchase tickets. When tickets are bought, this will be reflected and recorded in the application.

#### i. Features of selected project
The project involves the development of a comprehensive event management system with the following key features:

##### QR Code Scanner for Tickets
- Implementing a QR code scanning feature to help the festival organizers and staff keep track of tickets that have been used, and the type of ticket used (i.e. single day, 3-day ticket, etc.)

##### Ticketing Mangement
- Provide the capability to handle ticket-related operations, such as issuing refunds, transferring tickets, and generating reports. Tickets purchased will be recorded in the database with several useful attributes (i.e. scanned/not scanned, ticket type, amount paid, date purchased, etc.) 

##### Artist Mangement
- Develop an artist management module to keep track of artists that will be performing at the event. This encompasses creating and maintaining their profiles, managing schedules, and documenting performance details. 


##### Scheduling
- The system will assist in scheduling by meticulously organizing events and sets to ensure a seamless flow and timing, resulting in an engaging and enjoyable experience for all attendees.


##### Stage/Production Requirements
- Event organizers can outline technical and logistical stage requirements, ensuring the venue meets the performers’ needs. This streamlines planning for a seamless event experience. 

##### Vendors at Event
- Implement a vendor management system to handle contracts, payments, and logistics for vendors participating in events. This will also keep track of external vendors that are present at the event.

##### Map of Event using Google API
- Utilize the Google Maps API to provide an interactive map displaying the layout of the event venue, including stage locations, vendor booths, and facilities.

##### Worker and Organizer Authentication
- Organizer Role: Organizers will have admin permissions, granting them full control over the application. They can view, edit, and manage all aspects of the system.
- Worker Role: Workers will have viewing permissions for specific features within the application. They can interact with these features but are restricted from making administrative changes.

#### ii. End Users
The application will have two types of end users which are as follows:
- Event Organizers
- Event Staff

The functionalities of each end user within the app will depend on the permission granted for that user (either organizer/admin or staff).

#### iii. Integration of the end users with the project (user stories)
- As an event organizer, I want to see up-to-date data on ticket sales and attendee demographics to make informed decisions about the festival
- As an event organizer, I want to register artist/band information, including biographies and performance schedules
- As an event organizer, I want to manage production requirements such as stage setups, sound equipment and lighting
- As an event organizer, I want to communicate with staff and attendees by sending updates and announcements about the festival
- As event staff, I want to check in event-goers and scan their tickets using a QR scanner that connects to the application
- As event staff, I want to have a digital map of the festival grounds to help event-goers with directions and information
- As event staff, I want to see attendance data and check attendance at events or stages

#### iv. Areas covered by this project
- **Event Planning and Scheduling:** Creating festival schedules, registering performer information, coordinating productions requirements
- **Ticketing and Sales:** Managing ticket sales, record all data regarding ticket sales
- **Staff and Volunteer Management:** Assigning roles and tasks to event staff and volunteers
- **Map and Directional Services:** Providing digital maps of the festival grounds
- **Technical Production Management:** Coordinating technical requirements, managing stage equipment 

## Project Users, Actors, Vendors and Actuators

#### i. Users:
- Event organizers: Individuals who are hosting the event, or who are helping to organize the event at a managerial level.
- Event staff: Individuals who are working or volunteering at the event.

#### ii. Actors:
- Event Attendees: Purchase tickets and access event information through the festival’s website, as well as use scannable tickets for entry
- Ticketing and payment providers: Third-party ticketing and payment processing services will facilitate the purchase of tickets and secure payment.


#### iii. Vendors:
- Cloud Hosting Provider
- Database Management Service (PostgreSQL)

#### iv. Actuators:
- QR/RFID Scanners: Used to scan wristbands for entry control and tracking attendees
- Ticket Printers: To print physical tickets for attendees who weren’t able to get a digital version (i.e. bought at door)



## Project Properties
For this project we choose to create our desktop application using C# on the .NET framework. As per the assignment instructions we will develop our application which includes the screen and backend using C#. Additionally, to store all the event information in our application we will be using PostgreSQL instead of MS Server. Despite MS Server being easier to incorporate within the application due to the Microsoft tool set, we are more comfortable and familiar with PostgreSQL. 

In addition to the MS tool set already included within Visual Studio we will also need to import a C# QR Reader, Google Maps API, and Npgsql to configure PostgreSQL to work natively on the .NET ecosystem. Git and Github will be used for version control and collaboration and to set up our codebase. In order to avoid any conflicts in the code, separate branches will be used by each team member while implementing features. Once completed, changes will be merged into the main branch.
-  **Language**: C#
-  **Database**: PostgreSQL
- **Tools/Framework:** .NET, C# QR Scanner, Npgsql, Google Map API
- **Version Control:** Git, Github (jelmourne/cosmic-management-system)
