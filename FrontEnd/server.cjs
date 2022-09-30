
const bodyParser = require('body-parser');
const express = require('express');
const http = require('http');
const ejs = require('ejs');

const router = express.Router();


// Initialise Express
var app = express();
// Render static files
app.use(express.static('public'));
// Set the view engine to ejs
app.set('view engine', 'ejs');



app.use(bodyParser.urlencoded({ extended: false }));
app.use(bodyParser.json());

const delay = ms => new Promise(resolve => setTimeout(resolve, ms))





router.post('/api',async (req,res) => 
{
    console.log("Req for:"+JSON.stringify(req.body));
    
    let data = "Hello World";

    console.log('data returned: '+ JSON.stringify(data));

    res.json(data); 
});



const port = process.env.PORT || 3000;

const server = http.createServer(app);


server.listen(port);

// *** GET Routes - display pages ***
app.get('/',(req, res) => {
    res.redirect('/capture');
});

app.get('/capture',(req, res) =>{
    res.status(200).render('pages/capture');
});

app.get('/list', (req, res) => {
    res.render('pages/requests');
});




app.use("/", router);

app.use((req, res) => 
{
    //404 section here
    res.status(404).render('pages/404.ejs')
})