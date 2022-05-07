import React, {useEffect, useState} from 'react';
import {PageProps} from "gatsby";
import {request} from "../api/request";
import Layout from "../components/Layout";
import {Box, Button, FormControl, InputLabel, MenuItem, Paper, Select, TextField, Typography} from "@mui/material";
import {DatePicker, LocalizationProvider} from '@mui/x-date-pickers';
import {AdapterDateFns} from "@mui/x-date-pickers/AdapterDateFns";


const Trips = ({location}: PageProps) => {
  const [data, setData] = useState<any[]>();
  useEffect(() => {
    request('GET', '/Destinations').then(data => {
      setData(data.destinations)
    })
  }, [])
  const {state = {}} = location;
  // @ts-ignore
  const [destination, setDestination] = useState(state?.destination ?? '')
  const [departureDate, setDepartureDate] = useState<Date | undefined>(undefined);
  const [numberOfDays, setNumberOfDays] = useState<string>('')
  const [numberOfAdults, setNumberOfAdults] = useState<string>('')
  const [numberOfChildrenUpTo18, setNumberOfChildrenUpTo18] = useState<string>('')
  const [numberOfChildrenUpTo10, setNumberOfChildrenUpTo10] = useState<string>('')
  const [numberOfChildrenUpTo3, setNumberOfChildrenUpTo3] = useState<string>('')

  const [trips, setTrips] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(false);

  const loadTrips = () => {
    setLoading(true);
    if (destination && departureDate && numberOfDays && numberOfAdults) {
      request('GET', '/trips', {
        AirportCode: destination,
        DepartureDate: departureDate?.toISOString().split('T')[0],
        NumberOfDays: numberOfDays,
        NumberOfAdults: numberOfAdults,
      }).then((data) => {
        setLoading(false);
        console.log(data);
        setTrips(data?.trips);
      })
    } else {
      setError(true);
      setLoading(false);
    }
  }

  return (
    <Layout>
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
                  setDepartureDate(newValue ?? undefined);
                }}
                renderInput={(params) => <TextField {...params} required error={error} />}
              />
            </LocalizationProvider>
            <TextField
              required
              error={error}
              id="outlined-number"
              label="Number of days"
              type="number"
              value={numberOfDays}
              onChange={(event) => setNumberOfDays(event.target.value)}
            />
            <TextField
              required
              error={error}
              id="outlined-number"
              label="Number of adults"
              type="number"
              value={numberOfAdults}
              onChange={(event) => setNumberOfAdults(event.target.value)}
            />
            <TextField
              id="outlined-number"
              label="Children up to 18"
              type="number"
              value={numberOfChildrenUpTo18}
              onChange={(event) => setNumberOfChildrenUpTo18(event.target.value)}
            />
            <TextField
              id="outlined-number"
              label="Children up to 10"
              type="number"
              value={numberOfChildrenUpTo10}
              onChange={(event) => setNumberOfChildrenUpTo10(event.target.value)}
            />
            <TextField
              id="outlined-number"
              label="Children up to 3"
              type="number"
              value={numberOfChildrenUpTo3}
              onChange={(event) => setNumberOfChildrenUpTo3(event.target.value)}
            />
            <Box sx={{display: 'flex', justifyContent: 'flex-end', p: 2}}>
              <Button variant="contained" size="large" onClick={loadTrips}>Search Trips</Button>
            </Box>
          </Box>
          <Box>

          </Box>
        </Paper>
      </Box>
    </Layout>
  );
};

export default Trips;