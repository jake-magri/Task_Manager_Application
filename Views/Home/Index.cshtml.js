const taskData = {
    TaskName: "Sample Task",
    TaskDes: "This is a sample task",
    DueDate: "2023-09-16", // Format as needed
    Status: "Open"
};

fetch("/api/tasks", {
    method: "POST",
    headers: {
        "Content-Type": "application/json"
    },
    body: JSON.stringify(taskData)
})
    .then(response => {
        if (response.status === 201) {
            // Task created successfully
            return response.json();
        } else if (response.status === 400) {
            // Handle validation errors or other client-side errors
            return response.json();
        } else {
            // Handle other server errors (e.g., 500 Internal Server Error)
            throw new Error("Task creation failed");
        }
    })
    .then(data => {
        // Handle the response data, e.g., display a success message
        console.log("Task has been sent!");
    })
    .catch(error => {
        console.error("Error:", error);
    });
