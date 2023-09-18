@startuml
entity ticket {
id
holder
qr
}

entity user {
username
password
}

entity staff {
name 
department
admin
}

entity artist {
name
genre
}

entity set {
stage
schedule
requirement
artist
}

entity vendor {
name
income
}

entity customer {
name
}

artist "*"--"*" set

customer "*"-"*" vendor

customer "1"--"*" ticket

user - staff

user -- customer
@enduml
