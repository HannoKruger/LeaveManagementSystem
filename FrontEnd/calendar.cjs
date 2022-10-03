const {google} = require('googleapis');
require('dotenv').config();

// Provide the required configuration
const CREDENTIALS = JSON.parse(process.env.CREDENTIALS);
const calendarId = process.env.CALENDAR_ID;

// Google calendar API settings
const SCOPES = 'https://www.googleapis.com/auth/calendar';
const calendar = google.calendar({version : "v3"});

const auth = new google.auth.JWT(
    CREDENTIALS.client_email,
    null,
    CREDENTIALS.private_key,
    SCOPES
);

Date.prototype.addDays = function(days) {
    var date = new Date(this.valueOf());
    date.setDate(date.getDate() + days);
    return date;
}
function subtractHours(numOfHours, date = new Date()) {
    date.setHours(date.getHours() - numOfHours);
  
    return date;
  }
  


const TIMEOFFSET = '+00:00';

// Get date-time string for calender
const dateTimeForCalander = (start,end) => {

    let date = start;

    let year = date.getFullYear();
    let month = date.getMonth() + 1;
    if (month < 10) {
        month = `0${month}`;
    }
    let day = date.getDate();
    if (day < 10) {
        day = `0${day}`;
    }
    let hour = date.getHours();
    if (hour < 10) {
        hour = `0${hour}`;
    }
    let minute = date.getMinutes();
    if (minute < 10) {
        minute = `0${minute}`;
    }

    let newDateTime = `${year}-${month}-${day}T${hour}:${minute}:00.000${TIMEOFFSET}`;

    let event = new Date(Date.parse(newDateTime));

    let startDate = event;

    startDate = subtractHours(2, startDate);
    
    let endDate = startDate.addDays((end-start)/(1000*60*60*24));
   
    return {
        'start': startDate,
        'end': endDate
    }
};

// Insert new event to Google Calendar
const insertEvent = async (event) => {

    try {
        let response = await calendar.events.insert({
            auth: auth,
            calendarId: calendarId,
            resource: event
        });
    
        if (response['status'] == 200 && response['statusText'] === 'OK') {
            return "Event added successfully";
        } else {
            return "Error adding event";
        }
    } catch (error) {
        console.log(`Error at insertEvent --> ${error}`);
        return "Error adding event";
    }
};

// Get all the events between two dates
const getEvents = async (dateTimeStart, dateTimeEnd) => {

    try {
        let response = await calendar.events.list({
            auth: auth,
            calendarId: calendarId,
            timeMin: dateTimeStart,
            timeMax: dateTimeEnd,
            timeZone: 'Asia/Kolkata'
        });
    
        let items = response['data']['items'];
        return items;
    } catch (error) {
        console.log(`Error at getEvents --> ${error}`);
        return 0;
    }
};


// Delete an event from eventID
const deleteEvent = async (eventId) => {

    try {
        let response = await calendar.events.delete({
            auth: auth,
            calendarId: calendarId,
            eventId: eventId
        });

        if (response.data === '') {
            return 1;
        } else {
            return 0;
        }
    } catch (error) {
        console.log(`Error at deleteEvent --> ${error}`);
        return 0;
    }
};


module.exports = {insertEvent,dateTimeForCalander, getEvents, deleteEvent};