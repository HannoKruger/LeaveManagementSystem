const bodyParser = require('body-parser');
const express = require('express');
const http = require('http');
const ejs = require('ejs');
const qs = require('querystring')
const request = require('request');
const router = express.Router();

const  calendar = require('./calendar.cjs');
const insertEvent = calendar.insertEvent;
const dateTimeForCalander = calendar.dateTimeForCalander;


// Initialise Express
var app = express();
// Render static files
app.use(express.static('public'));
// Set the view engine to ejs
app.set('view engine', 'ejs');



app.use(bodyParser.urlencoded({ extended: false }));
app.use(bodyParser.json());

const delay = ms => new Promise(resolve => setTimeout(resolve, ms))



router.post('/form-data', async (req, res) => 
{
    let dateTime = dateTimeForCalander(new Date(req.body.LeaveStartDate), new Date(req.body.LeaveEndDate));
    
    // Event for Google Calendar
    let event = {
        'summary': "Leave for:" + req.body.FirstName + " " + req.body.LastName + "Type:"+ req.body.LeaveType,
        'description': req.body.Message,
        'start': {
            'dateTime': dateTime['start'],
            'timeZone': 'Africa/Johannesburg'
        },
        'end': {
            'dateTime': dateTime['end'],
            'timeZone': 'Africa/Johannesburg'
        }
    };



    console.log("Req for:" + JSON.stringify(req.body));

    let data = "Sucess";

    request.post({
        headers: { 'content-type': 'application/json; charset=utf-8' },
        url: 'http://localhost:8080' + '/form-data',
        body: JSON.stringify(req.body)
    }, function (error, response, body)
    {
        console.log("Response from server: " + body);

    });


    insertEvent(event).then((res) => {
        console.log(res);
    })
    .catch((err) => {
        console.log(err);
    });



    //console.log('data returned: '+ JSON.stringify(data));

    res.json(data);
});


router.get('/leave', async (req, res) =>
{
    //console.log("Req for:" + req.query);

    request.get(
        {
            headers: { 'content-type': 'application/json; charset=utf-8' },
            url: 'http://localhost:8080' + '/leave',
            body: JSON.stringify(req.query)
        },
        function (error, response, body) 
        {
            console.log("body: "+body);
        
            response = "Sucess";

            res.json(body);
        });

});



const port = process.env.PORT || 3000;

const server = http.createServer(app);


server.listen(port);

// *** GET Routes - display pages ***
app.get('/', (req, res) =>
{
    res.redirect('/capture');
});

app.get('/capture', (req, res) =>
{
    res.status(200).render('pages/capture');
});



app.get('/list', (req, res) =>
{
    request.get(
        {
            headers: { 'content-type': 'text/plain' },
            url: 'http://localhost:8080' + '/leave-requests',
            body: JSON.stringify(req.query)
        },
        function (error, response, body) 
        {
            console.log("List response: "+response.statusCode);  
            

            let leaveRequests = JSON.parse(body);


            res.render('pages/requests', { leaveRequests: leaveRequests });
     
        });
});



app.use("/", router);

app.use((req, res) => 
{
    //404 section here
    res.status(404).render('pages/404.ejs')
})