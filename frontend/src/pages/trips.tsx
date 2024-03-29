import React, {useEffect, useState} from 'react';
import {navigate, PageProps} from "gatsby";
import {request} from "../api/request";
import Layout from "../components/Layout";
import {
  Box,
  Button,
  Card,
  CardActions,
  CardContent,
  CardMedia,
  FormControl,
  Grid,
  InputLabel,
  MenuItem,
  Paper,
  Select,
  TextField,
  Typography
} from "@mui/material";
import {DatePicker, LocalizationProvider} from '@mui/x-date-pickers';
import {AdapterDateFns} from "@mui/x-date-pickers/AdapterDateFns";
import {HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel} from "@microsoft/signalr";


const Trips = ({location}: PageProps<{}, any, { destination: { airportCode: string, name: string } } | any>) => {
  const randomImages: number[] = [];
  useEffect(() => {
    for (let i = 0; i < 7; i++)
      randomImages.push(Math.floor(Math.random() * i * 1000))
  }, [])
  const [data, setData] = useState<any[]>();
  useEffect(() => {
    request('GET', '/Destinations').then(data => {
      setData(data?.destinations)
    })
  }, [])
  const {state} = location;
  const [destination, setDestination] = useState(state?.destination?.airportCode ?? 'AYT')
  const [departureDate, setDepartureDate] = useState<Date | null>(new Date("2022-07-02"));
  const [numberOfDays, setNumberOfDays] = useState<string>('7')
  const [numberOfAdults, setNumberOfAdults] = useState<string>('1')
  const [departure, setDeparture] = useState('any')
  const [numberOfChildrenUpTo18, setNumberOfChildrenUpTo18] = useState<string>('')
  const [numberOfChildrenUpTo10, setNumberOfChildrenUpTo10] = useState<string>('')
  const [numberOfChildrenUpTo3, setNumberOfChildrenUpTo3] = useState<string>('')

  const [searched, setSearched] = useState(false);

  const [trips, setTrips] = useState<({
    hotelCode: string
    hotelName: string,
    minimalPrice: number
  })[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(false);

  useEffect(() => {
    setTrips([]);
  }, [destination, departureDate, numberOfDays, numberOfAdults, numberOfChildrenUpTo18, numberOfChildrenUpTo10, numberOfChildrenUpTo3, departure])

  const loadTrips = () => {
    setLoading(true);
    if (destination && departureDate && numberOfDays && numberOfAdults) {
      request('POST', '/Trips', {
        AirportCode: destination,
        DepartureDate: departureDate?.toISOString().split('T')[0],
        NumberOfDays: numberOfDays,
        NumberOfAdults: numberOfAdults,
        NumberOfChildrenUpTo18: numberOfChildrenUpTo18 ? numberOfChildrenUpTo18 : 0,
        NumberOfChildrenUpTo10: numberOfChildrenUpTo10 ? numberOfChildrenUpTo10 : 0,
        NumberOfChildrenUpTo3: numberOfChildrenUpTo3 ? numberOfChildrenUpTo3 : 0,
        DepartureAirportCode: departure && departure !== 'any' ? departure : null,
      }).then((data) => {
        setLoading(false);
        setTrips(data?.trips);
      }).finally(() => setSearched(true))
    } else {
      setError(true);
      setLoading(false);
    }
  }

  const [connection, setConnection] = useState<HubConnection | null>(null);

  const [counts, setCounts] = useState<{ [key: string]: number }>({})

  useEffect(() => {
    const hotelHubConnection = new HubConnectionBuilder()
      .withUrl(process.env.WEB_API_URL + 'hotelsHub')
      .configureLogging(LogLevel.Warning)
      .withAutomaticReconnect()
      .build();

    setConnection(hotelHubConnection);
  }, []);

  useEffect(() => {
    if (connection) {
      connection.start()
        .then(result => {
          console.log('Connected!');
          connection.on("HotelsResponse", (response: { counts: { [key: string]: number } }) => {
            setCounts(response.counts)
          })
          connection.invoke("GetForDestination", {Destination: destination});
        })
        .catch(e => console.log('Connection failed: ', e));
      return () => {
        connection.stop().then(() => {
          console.log("Connection stopped");
        });
      }
    }
  }, [connection])

  useEffect(() => {
    if (connection?.state === HubConnectionState.Connected) {
      connection.invoke("GetForDestination", {Destination: destination});
    }
  }, [destination, connection]);

  useEffect(() => {
    if (connection) {
      connection.off("HotelCountUpdate");
      connection.on("HotelCountUpdate", (response) => {
        setCounts({...counts, [response.hotelCode]: response.orderCount})
      })
    }
  }, [connection, counts])

  return (
    <Layout>
      <>
        <Box sx={{p: 4}}>
          <Paper sx={{p: 2}}>
            <Typography variant="h6">
              Choose offer details
            </Typography>
            <Box sx={{'& .MuiFormControl-root': {m: 1, minWidth: 200}}}>
              <FormControl>
                <InputLabel id="destination" required error={error}>Destination</InputLabel>
                <Select
                  error={error}
                  id="destination"
                  label="Destination"
                  value={destination}
                  onChange={(event) => setDestination(event.target.value)}
                >
                  {data?.sort((a, b) => a.name > b.name ? 1 : -1).map(destinationElement => (
                    <MenuItem value={destinationElement.airportCode} key={destinationElement.airportCode}>
                      {destinationElement.name}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
              <LocalizationProvider dateAdapter={AdapterDateFns}>
                <DatePicker
                  label="Departure date"
                  value={departureDate}
                  onChange={(newValue) => {
                    setDepartureDate(newValue);
                  }}
                  renderInput={(params) => <TextField {...params} required error={error}/>}
                />
              </LocalizationProvider>
              <TextField
                required
                error={error}
                id="outlined-number"
                label="Number of days"
                type="number"
                value={numberOfDays}
                onChange={(event) => parseInt(event.target.value) < 0 ? setNumberOfDays('0') : setNumberOfDays(event.target.value)}
              />
              <TextField
                required
                error={error}
                id="outlined-number"
                label="Number of adults"
                type="number"
                value={numberOfAdults}
                onChange={(event) => parseInt(event.target.value) < 1 ? setNumberOfAdults('1') : setNumberOfAdults(event.target.value)}
              />
              <FormControl>
                <InputLabel id="departure" required error={error}>Departure Airport</InputLabel>
                <Select
                  error={error}
                  id="departure"
                  label="Departure Airport"
                  value={departure}
                  onChange={(event) => setDeparture(event.target.value)}
                >
                  <MenuItem value='any' key='unknown'>
                    Any
                  </MenuItem>
                  <MenuItem value='WRO' key='WRO'>
                    Wrocław
                  </MenuItem>
                  <MenuItem value='WAW' key='WAW'>
                    Warszawa-Chopina
                  </MenuItem>
                  <MenuItem value='KTW' key='KTW'>
                    Katowice
                  </MenuItem>
                  <MenuItem value='GDN' key='GDN'>
                    Gdańsk
                  </MenuItem>
                  <MenuItem value='KRK' key='KRK'>
                    Kraków
                  </MenuItem>
                  <MenuItem value='LUZ' key='LUZ'>
                    Lublin
                  </MenuItem>
                </Select>
              </FormControl>
              <TextField
                id="outlined-number"
                label="Children up to 18"
                type="number"
                value={numberOfChildrenUpTo18}
                onChange={(event) => parseInt(event.target.value) < 0 ? setNumberOfChildrenUpTo18('0') : setNumberOfChildrenUpTo18(event.target.value)}
              />
              <TextField
                id="outlined-number"
                label="Children up to 10"
                type="number"
                value={numberOfChildrenUpTo10}
                onChange={(event) => parseInt(event.target.value) < 0 ? setNumberOfChildrenUpTo10('0') : setNumberOfChildrenUpTo10(event.target.value)}
              />
              <TextField
                id="outlined-number"
                label="Children up to 3"
                type="number"
                value={numberOfChildrenUpTo3}
                onChange={(event) => parseInt(event.target.value) < 0 ? setNumberOfChildrenUpTo3('0') : setNumberOfChildrenUpTo3(event.target.value)}
              />
              <Box sx={{display: 'flex', justifyContent: 'flex-end', p: 2}}>
                <Button variant="contained" size="large" onClick={loadTrips}>
                  Search Trips
                </Button>
              </Box>
            </Box>
          </Paper>
        </Box>
        {trips.length && Object.keys(counts).filter(key => counts[key]).length ? (
          <>
            <Typography variant="h4">
              Most popular
            </Typography>
            <Grid spacing={2} container sx={{mb: 2}}>
              {Object.keys(counts).filter(key => counts[key]).sort((a, b) => counts[a] < counts[b] ? 1 : -1).slice(0, 5).map((key, i) => {
                const trip = trips?.find(element => element.hotelCode === key)
                return trip && (
                  <Grid key={trip.hotelCode} item xs>
                    <Card sx={{minWidth: 200}}>
                      <CardMedia
                        component="img"
                        height="200"
                        image={`https://picsum.photos/200/200?random=${i}`}
                        alt="destination"
                      />
                      <CardContent>
                        <Typography gutterBottom variant="h5" component="div">
                          {trip.hotelName}
                        </Typography>
                        <Box sx={{display: 'flex', justifyContent: 'space-between'}}>
                          <Typography>From:</Typography>
                          <Typography sx={{textAlign: 'right', fontWeight: 'bold'}}>{trip.minimalPrice}§</Typography>
                        </Box>
                        {counts[trip.hotelCode] ? <Typography>
                          {`${counts[trip.hotelCode]} ${counts[trip.hotelCode] > 1 ? 'recent bookings' : 'recent booking'} in this hotel!`}
                        </Typography> : ''}
                      </CardContent>
                      <CardActions>
                        <Button size="small" onClick={() => navigate('/offer', {
                          state: {
                            airportCode: destination,
                            departureDate,
                            numberOfDays,
                            numberOfAdults,
                            numberOfChildrenUpTo18,
                            numberOfChildrenUpTo10,
                            numberOfChildrenUpTo3,
                            departure,
                            trip,
                          }
                        })}>
                          Check Offer
                        </Button>
                      </CardActions>
                    </Card>
                  </Grid>
                );
              })}
            </Grid>
            <Typography variant="h4">
              Other offers
            </Typography>
          </>
        ) : null}
        {trips.length ?
          <Grid spacing={2} container>
            {trips.map((trip, i) =>
              <Grid key={trip.hotelCode} item xs>
                <Card sx={{minWidth: 200}}>
                  <CardMedia
                    component="img"
                    height="200"
                    image={`https://picsum.photos/200/200?random=${i}`}
                    alt="destination"
                  />
                  <CardContent>
                    <Typography gutterBottom variant="h5" component="div">
                      {trip.hotelName}
                    </Typography>
                    <Box sx={{display: 'flex', justifyContent: 'space-between'}}>
                      <Typography>From:</Typography>
                      <Typography sx={{textAlign: 'right', fontWeight: 'bold'}}>{trip.minimalPrice}§</Typography>
                    </Box>
                  {counts[trip.hotelCode] ? <Typography>
                    {`${counts[trip.hotelCode]} ${counts[trip.hotelCode] > 1 ? 'recent bookings' : 'recent booking'} in this hotel!`}
                  </Typography> : ''}
                  </CardContent>
                  <CardActions>
                    <Button size="small" onClick={() => navigate('/offer', {
                      state: {
                        airportCode: destination,
                        departureDate,
                        numberOfDays,
                        numberOfAdults,
                        numberOfChildrenUpTo18,
                        numberOfChildrenUpTo10,
                        numberOfChildrenUpTo3,
                        departure,
                        trip,
                      }
                    })}>
                      Check Offer
                    </Button>
                  </CardActions>
                </Card>
              </Grid>
            )}
          </Grid> : ''}
      </>
    </Layout>
  );
};

export default Trips;