using System.Windows;
using Microsoft.Win32;

namespace HospitalAgent;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        // Initialize agent and register with service
        InitializeAgent();
    }

    private async void InitializeAgent()
    {
        // In production: Register with HospitalAgentService API
        // Send heartbeat every 30 seconds
        // Handle auto-updates
    }

    private void ScanQrCode_Click(object sender, RoutedEventArgs e)
    {
        // In production: Initialize camera and ZXing.Net QR scanner
        // Process QR token
        // Fetch patient data from PatientService API
        // Display data for verification
        
        ScanResult.Text = "✓ QR Code scanned successfully! Patient: John Doe";
    }

    private void SelectDocument_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = "PDF files (*.pdf)|*.pdf|Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png|All files (*.*)|*.*",
            Title = "Select Document"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            SelectedFile.Text = $"Selected: {System.IO.Path.GetFileName(openFileDialog.FileName)}";
            
            // In production: Upload to Azure Blob Storage
            // Process with Azure Form Recognizer OCR
            // Map extracted data to FHIR resources
            // Attach to patient record
        }
    }
}

