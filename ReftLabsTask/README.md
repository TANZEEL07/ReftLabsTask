# ReftLabsTask

This project gets user data from a public API (reqres.in). It also handles errors, saves data for faster access, and retries failed requests.

## How to Run the Project

1. **Download the project**:
   - Use GitHub or Visual Studio to download the project to your computer.

2. **Open the project**:
   - Open the project in **Visual Studio**.

3. **Build the project**:
   - In Visual Studio, click on **Build** and select **Build Solution**.

4. **Run the project**:
   - Click the **Run** button in Visual Studio to start the app.
   - The app will run at `https://localhost:YourPort/`.

5. **Test the API**:
   - Go to `https://localhost:YourPort/swagger` in your browser.
   - You can test these API calls:
     - `GET /api/users/{id}` – Get a user by ID.
     - `GET /api/users` – Get all users.

## Configuration

- You can change the API URL and cache time in the `appsettings.json` file.

## Running Tests

1. **Open the test project**:
   - Open the **ReftLabsTask.Tests** project in Visual Studio.

2. **Build the tests**:
   - Click **Build** and select **Build Solution**.

3. **Run the tests**:
   - Click **Test** and select **Run All Tests** to check if everything works.

## Features

- **Caching**: Saves data to make it faster next time.
- **Retry Logic**: Tries again if the request fails.
