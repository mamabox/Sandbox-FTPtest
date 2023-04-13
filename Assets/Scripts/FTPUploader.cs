/**
 * Source: ChatGPT Mar23Version - https://chat.openai.com/chat
 **/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.IO;
using Unity.VisualScripting;
using System;

public class FTPUploader : MonoBehaviour
{
    public string ftpServerUrl;
    public string ftpUserName;
    public string ftpPassword;
    public string sourceFolderPath;
    public string destinationFolderPath;

    void Start()
    {
        ftpServerUrl = "ftp://ftp.mamafatou.com:21";
        ftpUserName = "spageoftp@mamafatou.com";
        ftpPassword = "spageoFTPtest";
        sourceFolderPath = Path.Combine(Application.dataPath, "Data");
        //public string sourceFolderPath = "/Assets/Data";
        destinationFolderPath = "uploads/test1";

    Debug.Log("FolderPath = " + sourceFolderPath);
        UploadFolderToFTP();
    }

    public void UploadFolderToFTP()
    {
        // Get the files in the source folder
        string[] files = Directory.GetFiles(sourceFolderPath);

        // Create FTP request to check if the destination folder already exists
        FtpWebRequest ftpCheckRequest = (FtpWebRequest)WebRequest.Create(ftpServerUrl + "/" + destinationFolderPath);
        ftpCheckRequest.Method = WebRequestMethods.Ftp.ListDirectory;
        ftpCheckRequest.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
        ftpCheckRequest.UseBinary = true;
        ftpCheckRequest.KeepAlive = false;

        try
        {
            // Send FTP request to check if the destination folder already exists
            FtpWebResponse ftpCheckResponse = (FtpWebResponse)ftpCheckRequest.GetResponse();
            ftpCheckResponse.Close();

            // If the destination folder already exists, throw an exception
            throw new Exception("Destination folder already exists on the FTP server.");
        }
        catch (WebException ex)
        {
            // If the destination folder does not exist, create it
            FtpWebResponse ftpCreateResponse = (FtpWebResponse)((FtpWebRequest)WebRequest.Create(ftpServerUrl + "/" + destinationFolderPath)).GetResponse();
            ftpCreateResponse.Close();

            // Upload each file in the source folder to the FTP server
            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);

                // Create FTP request
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(ftpServerUrl + "/" + destinationFolderPath + "/" + fileInfo.Name);
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                ftpRequest.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
                ftpRequest.UseBinary = true;
                ftpRequest.KeepAlive = false;

                // Open the source file
                FileStream fileStream = fileInfo.OpenRead();

                // Get the destination stream
                Stream destinationStream = ftpRequest.GetRequestStream();

                // Copy the file to the destination stream
                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    destinationStream.Write(buffer, 0, bytesRead);
                }

                // Close the streams
                fileStream.Close();
                destinationStream.Close();

                // Get the FTP response
                FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpResponse.Close();
            }
        }
    }

}
