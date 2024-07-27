const chrono = require('chrono-node');
const moment = require('moment-timezone');

module.exports = async function (context, req) {
    context.log('DateTimeParser JavaScript HTTP trigger function processed a request.');

    const text = (req.body && req.body.text);
    context.log("req.body.text: " + text);

    const tz = (req.body && req.body.timezone);
    context.log("req.body.timezone: " + tz);

    var date = chrono.parseDate(text,  { timezone: tz }, {forwardDate: true});
    context.log("chrono.parseDate: " + date);

    const responseMessage = JSON.stringify({ "text": text, "timestamp": date }, null, " ")
    context.log("JSON.stringify(date): " + responseMessage);

    context.res = {
        // status: 200, /* Defaults to 200 */
        body: responseMessage
    };
}