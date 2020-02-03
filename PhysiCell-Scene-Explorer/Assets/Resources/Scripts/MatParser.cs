using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;

public class MatParser
{
	int rows, cols;
	string matName;
	float[,] data;
	
	// based on the read_matlab code found at:
	// https://github.com/MathCancer/PhysiCell/blob/master/BioFVM/BioFVM_matlab.cpp
	/*
	A. Ghaffarizadeh, S.H. Friedman, and P. Macklin, BioFVM: an efficient 
    parallelized diffusive transport solver for 3-D biological simulations,
    Bioinformatics 32(8): 1256-8, 2016. DOI: 10.1093/bioinformatics/btv730 
	*/
    public void parseMat(string filepath)
    {
		data = new float[0, 0];
		
		try{
			// make the file pointer
			var fs = new FileStream(filepath, FileMode.Open);
			
			// each mat file starts with a header
			// the header is 20 bytes + the length of the name
			//// the length of the matName is found in header bytes [17, 20]
			
			// read basic header info
			var buffer = new byte[sizeof(uint)];
			fs.Read(buffer, 0, sizeof(uint));
			int headerInfo = BitConverter.ToInt32(buffer, 0);
			int typeNumericFormat = thousands(headerInfo);
			int typeReserved = hundreds(headerInfo);
			int typeDataFormat = tens(headerInfo);
			int typeMatrixType = ones(headerInfo);
			
			// check that the file is the proper version (matlab L4)
			if(typeNumericFormat != 0 || typeReserved != 0 || typeDataFormat > 5 || typeMatrixType != 0){
				Debug.Log("ERROR: cannot read this version");
				return;
			}
			
			// get the size of the data
			fs.Read(buffer, 0, sizeof(uint));
			rows = BitConverter.ToInt32(buffer, 0);
			
			fs.Read(buffer, 0, sizeof(uint));
			cols = BitConverter.ToInt32(buffer, 0);
			
			
			// make sure there are no imaginary numbers
			fs.Read(buffer, 0, sizeof(uint));
			int imaginaryPresent = BitConverter.ToInt32(buffer, 0);
			if(imaginaryPresent != 0){
				Debug.Log("ERROR: cannot process imaginary numbers");
				return;
			}
			
			// read in the matNam length;
			fs.Read(buffer, 0, sizeof(uint));
			int mat_name_len = BitConverter.ToInt32(buffer, 0);
			
			// read in the matName
			buffer = new byte[mat_name_len];
			fs.Read(buffer, 0, mat_name_len);
			
			// assemble the matName into a string
			matName = "";	
			foreach(byte b in buffer){
				matName += Convert.ToChar(b);
			}

			// the header has been read
			// proceed to read the data
			
			// resize the data according to specified size
			data = new float[rows, cols];
			
			buffer = new byte[sizeof(double)];
			int i = 0, j = 0;
			for(int n = 0; n < rows * cols; n++){
				fs.Read(buffer, 0, sizeof(double));
				// Debug.Log("Current: " + i + "/" + rows + " , " + j + "/" + cols);
				// Debug.Log(BitConverter.ToDouble(buffer, 0));
				data[i, j] = (float) BitConverter.ToDouble(buffer, 0);
				
				/*
				if((i == 1 || i == 2 || i == 3) && data[i, j] >= 100000){
					Debug.Log(j + ": " + data[i, j] + ", " + BitConverter.ToDouble(buffer, 0) + ", " + buffer);
				}
				*/
				
				i++;
				if(i == rows){
					i = 0;
					j++;
				}
			}
			fs.Close();
		}
		catch(FileNotFoundException fe){
			Debug.Log("Error: could not find file " + filepath + "\n\n" + fe);
		}
    }

	// accessors
	public int getNumRows(){
		return rows;
	}
	public int getNumCols(){
		return cols;
	}
	public float[,] getData(){
		return data;
	}
	
	
	// utility functions
	int ones(int n){
		return n % 10;
	}
	int tens(int n){
		return (n / 10) % 10;
	}
	int hundreds(int n){
		return (n / 100) % 10;
	}
	int thousands(int n){
		return (n / 1000) % 10;
	}
}
