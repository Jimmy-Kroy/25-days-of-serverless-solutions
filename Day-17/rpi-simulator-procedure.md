# Procedure: Connecting Raspberry Pi Simulator to Azure IoT Hub

## Prerequisites

Before you begin, ensure you have:

1. **An active Azure subscription**
2. **An IoT Hub created in your Azure subscription**
   - If you don't have one, follow the steps at: [Create an IoT hub](https://learn.microsoft.com/en-us/azure/iot-hub/create-hub)
3. **A device registered in your IoT hub**
   - If you haven't registered a device yet, follow: [Register a device](https://learn.microsoft.com/en-us/azure/iot-hub/create-connect-device#register-a-device)

---

## Step-by-Step Connection Procedure

### Step 1: Obtain Your Device Connection String

1. Log in to the [Azure Portal](https://portal.azure.com)
2. Navigate to your IoT Hub resource
3. In the left menu, select **Devices** under the "Device management" section
4. Click on your registered device name
5. Copy the **Primary Connection String** (it looks like: `HostName=<your-iot-hub>.azure-devices.net;DeviceId=<device-id>;SharedAccessKey=<key>`)
6. Keep this connection string ready for the next step

### Step 2: Open the Raspberry Pi Web Simulator

1. Open your web browser
2. Navigate to: [https://azure-samples.github.io/raspberry-pi-web-simulator/#GetStarted](https://azure-samples.github.io/raspberry-pi-web-simulator/#GetStarted)
3. The simulator page will load with three main areas:
   - **Assembly area** (left): Visual representation of the Pi with BME280 sensor and LED
   - **Coding area** (middle): Code editor with the sample application
   - **Console window** (right): Output display

### Step 3: Configure the Connection String in the Simulator

1. In the **Coding area**, locate **line 15** which contains:
   ```javascript
   const connectionString = '[Your IoT hub device connection string]';
   ```

2. Replace the placeholder text `[Your IoT hub device connection string]` with your actual connection string from Step 1:
   ```javascript
   const connectionString = 'HostName=<your-iot-hub>.azure-devices.net;DeviceId=<device-id>;SharedAccessKey=<key>';
   ```

3. Ensure the connection string is enclosed in single quotes

### Step 4: Run the Application

You have two options to start the application:

**Option A:**
- Click the **Run** button at the top of the console window

**Option B:**
- Type `npm start` in the integrated console window and press Enter

### Step 5: Verify the Connection

After running the application, you should see output in the console window showing:

1. **Initialization messages** indicating the sensor and client are starting
2. **Sensor data readings** including:
   - Message ID
   - Device ID: "Raspberry Pi Web Client"
   - Temperature in Celsius
   - Humidity percentage
3. **Success messages** confirming data is being sent to Azure IoT Hub
4. **LED blinking** in the assembly area (visual indicator of message transmission)

The simulator sends data every 2 seconds by default.

### Step 6: Monitor Messages in Azure IoT Hub (Optional)

To verify messages are being received:

1. Return to the Azure Portal
2. Navigate to your IoT Hub
3. In the left menu, select **Overview**
4. Check the **Device to cloud messages** metric to see incoming telemetry
5. Alternatively, use Azure IoT Explorer or Azure CLI to monitor device-to-cloud messages

---

## Understanding the Simulator Components

### Assembly Area
- **BME280 Sensor**: Simulates a temperature and humidity sensor connected via I2C.1
- **LED**: Connected to GPIO 4, blinks when messages are sent/received

### Code Functionality
The sample application:
- Reads temperature and humidity from the simulated BME280 sensor
- Sends JSON messages to IoT Hub every 2 seconds
- Sets a temperature alert property if temperature exceeds 30°C
- Responds to cloud-to-device method calls (`start` and `stop`)
- Blinks the LED for 500ms when messages are sent or received

### Console Controls
- **Run**: Executes the application
- **Reset**: Restores the default sample code
- **Collapse/Expand**: Hides or shows the console window

---

## Troubleshooting

**Issue**: Connection error messages in console

**Solution**: 
- Verify your connection string is correct and properly formatted
- Ensure your device is registered in IoT Hub
- Check that your Azure subscription is active

**Issue**: No output in console

**Solution**:
- Click the Run button or type `npm start`
- Check browser console for JavaScript errors (F12)
- Try refreshing the page and entering the connection string again

**Issue**: Messages not appearing in IoT Hub

**Solution**:
- Confirm the device is registered and enabled in IoT Hub
- Check IoT Hub metrics for any throttling or errors
- Verify your connection string matches the registered device

---

## Additional Notes

- The Raspberry Pi web simulator is currently archived but remains functional for learning and testing
- The simulated sensor generates realistic temperature and humidity values
- You can modify the code in the coding area to customize behavior
- The application is compatible with physical Raspberry Pi devices running the same Node.js code