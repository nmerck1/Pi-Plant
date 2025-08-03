package main

import (
    "encoding/json"
    "fmt"
    "log"
    "net/http"
)

func enableCORS(w http.ResponseWriter) {
    w.Header().Set("Access-Control-Allow-Origin", "*")
    w.Header().Set("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS")
    w.Header().Set("Access-Control-Allow-Headers", "Content-Type")
}

func healthHandler(w http.ResponseWriter, r *http.Request) {
    enableCORS(w)
    w.Header().Set("Content-Type", "application/json")
    json.NewEncoder(w).Encode(map[string]string{"status": "ok", "message": "Backend is running"})
}

func main() {
    http.HandleFunc("/api/health", healthHandler)
    
    fmt.Println("Go backend server starting on :8080")
    log.Fatal(http.ListenAndServe(":8080", nil))
}