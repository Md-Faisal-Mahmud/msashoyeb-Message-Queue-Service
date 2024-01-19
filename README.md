# Message Queue Service (Online Shop Management)

## Overview

This document outlines the system design for a demo project implementing RabbitMQ as a message broker. The project includes the following requirements:

- At least two channels
- Two or more message publishers for each channel
- Two or more consumers for each channel
- Two shared consumers for both channels (work interchangeably)

## System Architecture

### RabbitMQ Setup

The system utilizes RabbitMQ as the message broker. Ensure RabbitMQ is installed and running.

### Channels

1. **Channel 1**
   - Two message publishers: `Publisher 1`, `Publisher 2`
   - Two consumers: `Consumer 1`, `Consumer 2`

2. **Channel 2**
   - Two message publishers: `Publisher 1`, `Publisher 2`
   - Two consumers: `Consumer 1`, `Consumer 2`

## Components and Modules

### Message Publishers

#### Publisher 1 (Channel 1)

- Responsible for publishing order messages from "Channel 1" to the message broker.

#### Publisher 2 (Channel 1)

- Another publisher responsible for publishing order messages from "Channel 1" to the message broker.

#### Publisher 1 (Channel 2)

- Responsible for publishing inventory messages from "Channel 2" to the message broker.

#### Publisher 2 (Channel 2)

- Another publisher responsible for publishing inventory messages from "Channel 2" to the message broker.

### Message Consumers

#### Consumer 1 (Channel 1)

- Consumes messages from "Channel 1".

#### Consumer 2 (Channel 1)

- Another consumer for "Channel 1".

#### Consumer 1 (Channel 2)

- Consumes messages from "Channel 2".

#### Consumer 2 (Channel 2)

- Another consumer for "Channel 2".

### Shared Consumers

#### Shared Consumer 1

- Consumes messages from both "Channel 1" and "Channel 2".

#### Shared Consumer 2

- Another shared consumer for both channels.

## Technologies Used

- RabbitMQ (Version 3.12.12) and Erlang (Version 26.2.1)

## Data Flow

- Diagrams illustrating the flow of messages within each channel.

![Screenshot 2024-01-18 234620](https://github.com/msashoyeb/Message-Queue-Service/assets/53036465/b373e7e9-5416-4e1a-a10f-0b5a3358239f)
