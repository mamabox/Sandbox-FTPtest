using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

/**
 * Source: https://github.com/mminer/ftp-uploader-unity/blob/master/Editor/Uploader.cs
 **/

public class FTPUploadFile : MonoBehaviour
{
    private string FTPHost = "ftp.mamafatou.com";
    private string folderPath = "uploads";
    private int port = 21;

    private string FTPPath = "ftp://ftp.mamafatou.com:21/public_ftp/spageoftp";
    private string FTPUserName = "spageoftp@mamafatou.com";
    private string FTPPwd = "spageoFTPtest";

    private string filePath = "Assets";
    private string filenameRead = "testRead.txt";
    private string filenameWrite = "testWrite.txt";

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("FTP READ");
        UploadTest(filenameWrite, FTPHost, FTPUserName, FTPPwd, folderPath);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UploadTest(string filename, string server, string username, string password, string initialPath)
    {
        var file = new FileInfo(filename);
        var temp = "ftp://" + server + ":" + port + "/" + initialPath + "/" + file.Name;
        Debug.Log("URI = " + temp);
        var address = new Uri(temp);
        var request = FtpWebRequest.Create(address) as FtpWebRequest;

        // Upload options:

        // Provide credentials
        request.Credentials = new NetworkCredential(username, password);

        // Set control connection to closed after command execution
        request.KeepAlive = false;

        // Specify command to be executed
        request.Method = WebRequestMethods.Ftp.UploadFile;

        // Specify data transfer type
        request.UseBinary = true;

        // Notify server about size of uploaded file
        request.ContentLength = file.Length;

        // Set buffer size to 2KB.
        var bufferLength = 2048;
        var buffer = new byte[bufferLength];
        var contentLength = 0;

        // Open file stream to read file
        var fs = file.OpenRead();

        try
        {
            // Stream to which file to be uploaded is written.
            var stream = request.GetRequestStream();

            // Read from file stream 2KB at a time.
            contentLength = fs.Read(buffer, 0, bufferLength);

            // Loop until stream content ends.
            while (contentLength != 0)
            {
                //Debug.Log("Progress: " + ((fs.Position / fs.Length) * 100f));
                // Write content from file stream to FTP upload stream.
                stream.Write(buffer, 0, contentLength);
                contentLength = fs.Read(buffer, 0, bufferLength);
            }

            // Close file and request streams
            stream.Close();
            fs.Close();
        }
        catch (Exception e)
        {
            Debug.LogError("Error uploading file: " + e.Message);
            return;
        }

        Debug.Log("Upload successful.");
    }
}
