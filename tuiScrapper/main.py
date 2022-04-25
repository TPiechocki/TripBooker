import requests
import json
import csv

dest_codes = ["PT", "VRA", "MLE", "HR", "DO", "AUA", "CHQ", "NBE", "PFO", "CMB", "SPU", "DBV", "IC", "ID",
              "AYT", "BOJ", "CUN", "TFS", "CUR", "EG", "ZNZ", "LCA", "BCN", "SMI", "DXB", "SID", "HKT", "TGD",
              "GMZ", "MAH", "RKT", "LPA", "BG", "DPS", "FXX", "SEZ", "JTR", "MBA", "ZTH", "MRU", "CFU", "MBJ",
              "FUE", "RHO", "IBZ", "ACE", "PUJ", "SSH", "AGP", "RMF", "PMI", "TIA", "SKG", "CU", "FAO", "KGS",
              "CY", "HEV", "GPA", "TN", "FNC", "HRG", "DJE", "ADB"]

connections = set()
hotels = set()

for code in dest_codes:
    results = 500
    page = 0
    while True:
        json_data = {
            "childrenBirthdays": [],
            "departuresCodes": [],
            "destinationsCodes": [code],
            "durationFrom": 1,
            "durationTo": 20,
            "numberOfAdults": 2,
            "offerType": "BY_PLANE",
            "filters": [
                {
                    "filterId": "priceSelector",
                    "selectedValues": []
                },
                {
                    "filterId": "board",
                    "selectedValues": []
                },
                {
                    "filterId": "amountRange",
                    "selectedValues": []
                },
                {
                    "filterId": "minHotelCategory",
                    "selectedValues": [
                        "defaultHotelCategory"
                    ]
                },
                {
                    "filterId": "tripAdvisorRating",
                    "selectedValues": [
                        "defaultTripAdvisorRating"
                    ]
                },
                {
                    "filterId": "beach_distance",
                    "selectedValues": [
                        "defaultBeachDistance"
                    ]
                },
                {
                    "filterId": "facilities",
                    "selectedValues": []
                },
                {
                    "filterId": "WIFI",
                    "selectedValues": []
                },
                {
                    "filterId": "sport_and_wellness",
                    "selectedValues": []
                },
                {
                    "filterId": "room_type",
                    "selectedValues": []
                },
                {
                    "filterId": "airport_distance",
                    "selectedValues": []
                },
                {
                    "filterId": "flight_category",
                    "selectedValues": []
                },
                {
                    "filterId": "additionalType",
                    "selectedValues": []
                }
            ],
            "metaData": {
                "page": page,
                "pageSize": results,
                "sorting": "price"
            }
        }
        headers = {
            'Content-Type': 'application/json',
            #'Cookie': 'destinations=""; childrenCount=0; adultsCount=2; searchChildList=""; travelType=WS; airports=""; testab=B; tuiLastSearch=d78492f3-26d3-468e-9db9-29b51a853ed0; JSESSIONID=web9~2B2D03210CF808C97D0A513EF2840A0B',
            'Connection': 'keep-alive',
            'Accept-Encoding': 'gzip, deflate, br',
            'Accept': '*/*',
            'User-Agent': 'PostmanRuntime/7.26.8'
        }
        resp = requests.post("https://www.tui.pl/search/offers", json=json_data, headers=headers).json()

        page += 1
        for info in resp["offers"]:
            country = info["breadcrumbs"][0]["label"]
            pl_airport_name = info["departureFlight"]["departure"]["airportName"]
            pl_airport_code = info["departureFlight"]["departure"]["airportCode"]
            foreign_airport_name = info["departureFlight"]["arrival"]["airportName"]
            foreign_airport_code = info["departureFlight"]["arrival"]["airportCode"]

            duration = info["departureFlight"]["flightDuration"]
            duration_hours, duration_minutes = duration.split(" ")[0].strip("h"), duration.split(" ")[1].strip("min")
            duration_total = int(duration_hours) * 60 + int(duration_minutes)

            connections.add((pl_airport_name, pl_airport_code, "Polska", foreign_airport_name, foreign_airport_code, country, duration_total))
            connections.add((foreign_airport_name, foreign_airport_code, country, pl_airport_name, pl_airport_code, "Polska", duration_total))

            hotel_name = info["hotelName"]
            hotel_code = info["hotelCode"]
            hotel_rating = info["tripAdvisorRating"] if "tripAdvisorRating" in info else "No rating"
            hotel_features = ", ".join(info["features"])
            hotels.add((country, hotel_name, hotel_code, foreign_airport_code, hotel_rating, hotel_features))

        if page == resp["pagination"]["pagesCount"]:
            break


cw = csv.writer(open("flights.csv", 'w', newline='', encoding="utf-8"))
cw.writerow(["origin_airport_name","origin_airport_code","origin_airport_country","destination_airport_name","destination_airport_code","destination_airport_country","duration_min"])
cw.writerows(list(connections))

cw_hotels = csv.writer(open("hotels.csv", 'w', newline='', encoding="utf-8"))
cw_hotels.writerow(["country","hotel_name","hotel_code","airport_code","hotel_rating","hotel_features"])
cw_hotels.writerows(list(hotels))

print(connections)
print(hotels)


