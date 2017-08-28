using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MyDate {

	public int year;
	public int month;
	public int day;

	static string[] monthNames = new string[12] {
 		"Gennaio", "Febbraio", "Marzo","Aprile",
		 "Maggio", "Giugno", "Luglio", "Agosto",
		 "Settembre", "Ottobre", "Novembre", "Dicembre"
 	};

	public MyDate() {
		var date = DateTime.Now;
		year = date.Year;
		month = date.Month;
		day = date.Day;
	}

	public MyDate(int year, int month, int day) {
		this.year = year;
		this.month = month;
		this.day = day;
	}

	public static string GetMonthName(int monthNumber) {
		if (monthNumber < 1 || monthNumber > 12)
			return "Error";

		return monthNames[monthNumber - 1];
	}

	public DateTime GetDateTime() {
		return new DateTime(year, month, day);
	}

	public bool IsGTE(MyDate date) {
		return new DateTime(year, month, day) >= new DateTime(date.year, date.month, date.day);
	}

	public bool IsMonthGTE(int year, int month) {
		return new DateTime(this.year, this.month, 1) >= new DateTime(year, month, 1);
	}

}
