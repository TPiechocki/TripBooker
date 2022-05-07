import React, {useEffect, useState} from 'react';
import {PageProps} from "gatsby";
import Layout from "../components/Layout";
import {
  Alert,
  Box, Button,
  Card,
  CardMedia,
  FormControl,
  InputLabel,
  MenuItem,
  Paper,
  Select, Snackbar,
  TextField,
  Typography
} from "@mui/material";
import {request} from "../api/request";

interface TransportOption {
  availablePlaces: number,
  departureAirportCode: string,
  departureAirportName: string,
  departureDate: string,
  destinationAirportCode: string,
  destinationAirportName: string,
  id: string,
  ticketPrice: number,
  type: string
}

interface Options {
  hotelDays: string[],
  transportOptions: TransportOption[],
  returnTransportOptions: TransportOption[],
  hotelAvailability: {
    id: string,
    airportCode: string,
    allInclusive: boolean,
    allInclusivePrice: number,
    apartmentPrice: number,
    breakfastPrice: number,
    country: string,
    date: string,
    hotelCode: string,
    hotelId: string,
    hotelName: string,
    largePrice: number,
    mediumPrice: number,
    rating: number,
    roomsApartment: number,
    roomsLarge: number,
    roomsMedium: number,
    roomsSmall: number,
    roomsStudio: number,
    smallPrice: number,
    studioPrice: number
  }
}

interface State {
  airportCode: string,
  departureDate: Date,
  numberOfDays: string,
  numberOfAdults: string,
  numberOfChildrenUpTo18: string,
  numberOfChildrenUpTo10: string,
  numberOfChildrenUpTo3: string,
  departure: string,
  trip: {
    hotelCode: string
    hotelName: string,
    minimalPrice: number
  }
}

const Offer = ({location}: PageProps<{}, any, State | any>) => {
  const [open, setOpen] = React.useState(false);
  const {state} = location;
  const [options, setOptions] = useState<Options | null>(null);
  useEffect(() => {
    request('POST', '/Trip/Options', {
      HotelCode: state.trip.hotelCode,
      DepartureDate: state.departureDate?.toISOString().split('T')[0],
      NumberOfDays: state.numberOfDays,
      NumberOfAdults: state.numberOfAdults
    }).then(data => {
      setOptions(data)
    })
  }, [])
  const [numberOfAdults, setNumberOfAdults] = useState<string>(state.numberOfAdults)
  const [departure, setDeparture] = useState(state.departure ? state.departure : 'individual')
  const [arrival, setArrival] = useState(state.departure ? state.departure : 'individual')
  const [numberOfChildrenUpTo18, setNumberOfChildrenUpTo18] = useState<string>(state.numberOfChildrenUpTo18)
  const [numberOfChildrenUpTo10, setNumberOfChildrenUpTo10] = useState<string>(state.numberOfChildrenUpTo10)
  const [numberOfChildrenUpTo3, setNumberOfChildrenUpTo3] = useState<string>(state.numberOfChildrenUpTo3)
  const [mealOption, setMealOption] = useState(options?.hotelAvailability.allInclusive ? '2' : '1');
  const [discount, setDiscount] = useState('');
  const [numberOfStudioRooms, setNumberOfStudioRooms] = useState<string>('')
  const [numberOfSmallRooms, setNumberOfSmallRooms] = useState<string>('')
  const [numberOfMediumRooms, setNumberOfMediumRooms] = useState<string>('')
  const [numberOfLargeRooms, setNumberOfLargeRooms] = useState<string>('')
  const [numberOfApartmentRooms, setNumberOfApartmentRooms] = useState<string>('')

  const [priceData, setPriceData] = useState<{ validationError?: string | null, isAvailable: boolean, finalPrice: number } | null>(null);

  useEffect(() => {
    request('POST', '/Trip', {
      Flights: [
        departure !== 'individual' ? options?.transportOptions.find((transport) => transport.destinationAirportCode === departure) : null,
        arrival !== 'individual' ? options?.returnTransportOptions.find((transport) => transport.destinationAirportCode === arrival) : null,
      ].filter(element => element !== null),
      HotelDays: options?.hotelDays,
      NumberOfAdults: numberOfAdults,
      NumberOfChildrenUpTo18: numberOfChildrenUpTo18 ? numberOfChildrenUpTo18 : 0,
      NumberOfChildrenUpTo10: numberOfChildrenUpTo10 ? numberOfChildrenUpTo10 : 0,
      NumberOfChildrenUpTo3: numberOfChildrenUpTo3 ? numberOfChildrenUpTo3 : 0,
      NumberOfStudios: numberOfStudioRooms ? numberOfStudioRooms : 0,
      NumberOfSmallRooms: numberOfSmallRooms ? numberOfSmallRooms : 0,
      NumberOfMediumRooms: numberOfMediumRooms ? numberOfMediumRooms : 0,
      NumberOfLargeRooms: numberOfLargeRooms ? numberOfLargeRooms : 0,
      NumberOfApartmentRooms: numberOfApartmentRooms ? numberOfApartmentRooms : 0,
      MealOption: mealOption,
    }).then(data => {
      if (data?.validationError) {
        setOpen(true);
      } else {
        setOpen(false)
      }
      setPriceData(data);
    })
  }, [arrival, departure, mealOption, numberOfAdults, numberOfApartmentRooms, numberOfChildrenUpTo10, numberOfChildrenUpTo18, numberOfChildrenUpTo3, numberOfLargeRooms, numberOfMediumRooms, numberOfSmallRooms, numberOfStudioRooms, options?.hotelDays, options?.returnTransportOptions, options?.transportOptions])

  return (
    <Layout>
      <>
        <Box sx={{py: 2}}>
          <Typography variant="h5" sx={{mb: 2}}>{state.trip.hotelName}</Typography>
          <Box sx={{display: 'flex'}}>
            <Paper sx={{flexBasis: 500, flexGrow: 1, p: 2, mr: 2}}>
              <Typography variant="h5">Options</Typography>
              <Typography color="text.secondary">All prices are approximate</Typography>
              <Box
                sx={{display: 'flex', flexWrap: 'wrap', '& .MuiFormControl-root': {m: 1, minWidth: 200, flexGrow: 1}}}>
                <TextField
                  id="outlined-number"
                  label="Departure date"
                  value={state.departureDate?.toISOString().split('T')[0]}
                  variant="standard"
                  disabled
                />
                <TextField
                  id="outlined-number"
                  label="Number of days"
                  value={state.numberOfDays}
                  variant="standard"
                  disabled
                />
                <FormControl fullWidth>
                  <InputLabel id="departure" required>Departure Airport</InputLabel>
                  <Select
                    id="departure"
                    label="Departure Airport"
                    value={departure}
                    onChange={(event) => setDeparture(event.target.value)}
                  >
                    <MenuItem value='individual' key='individual'>
                      Individual transport
                    </MenuItem>
                    {options?.transportOptions.map(transport => (
                      <MenuItem value={transport.destinationAirportCode} key={transport.destinationAirportCode}>
                        {transport.destinationAirportName} from {transport.ticketPrice}§/place
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
                <FormControl fullWidth>
                  <InputLabel id="arrival" required>Arrival Airport</InputLabel>
                  <Select
                    id="arrival"
                    label="Arrival Airport"
                    value={arrival}
                    onChange={(event) => setArrival(event.target.value)}
                  >
                    <MenuItem value='individual' key='individual'>
                      Individual transport
                    </MenuItem>
                    {options?.returnTransportOptions.map(transport => (
                      <MenuItem value={transport.destinationAirportCode} key={transport.destinationAirportCode}>
                        {transport.destinationAirportName} from {transport.ticketPrice}§/place
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
                <TextField
                  required
                  id="outlined-number"
                  label="Number of adults"
                  type="number"
                  value={numberOfAdults}
                  onChange={(event) => parseInt(event.target.value) < 0 ? setNumberOfAdults('0') : setNumberOfAdults(event.target.value)}
                />
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
                <FormControl fullWidth>
                  <InputLabel id="arrival" required>Meals</InputLabel>
                  <Select
                    id="arrival"
                    label="Meals"
                    value={mealOption}
                    onChange={(event) => setMealOption(event.target.value)}
                  >
                    <MenuItem value='0' key='0'>
                      No meals
                    </MenuItem>
                    <MenuItem value='1' key='1'>
                      Breakfast {options?.hotelAvailability.breakfastPrice}§/person
                    </MenuItem>
                    {options?.hotelAvailability.allInclusive && <MenuItem value='2' key='2'>
                      All Inclusive {options.hotelAvailability.allInclusivePrice}§/person
                    </MenuItem>}
                  </Select>
                </FormControl>
                <Typography variant="h6" sx={{width: '100%'}}>Choose rooms</Typography>
                <TextField
                  id="outlined-number"
                  label={`Studio rooms ${options?.hotelAvailability.studioPrice.toFixed(2)}§`}
                  type="number"
                  value={numberOfStudioRooms}
                  error={parseInt(numberOfStudioRooms) > (options?.hotelAvailability.roomsStudio || 0)}
                  onChange={(event) => parseInt(event.target.value) < 0 ? setNumberOfStudioRooms('0') : setNumberOfStudioRooms(event.target.value)}
                />
                <TextField
                  id="outlined-number"
                  label={`Small rooms ${options?.hotelAvailability.smallPrice.toFixed(2)}§`}
                  type="number"
                  value={numberOfSmallRooms}
                  error={parseInt(numberOfSmallRooms) > (options?.hotelAvailability.smallPrice || 0)}
                  onChange={(event) => parseInt(event.target.value) < 0 ? setNumberOfSmallRooms('0') : setNumberOfSmallRooms(event.target.value)}
                />
                <TextField
                  id="outlined-number"
                  label={`Medium rooms ${options?.hotelAvailability.mediumPrice.toFixed(2)}§`}
                  type="number"
                  value={numberOfMediumRooms}
                  error={parseInt(numberOfMediumRooms) > (options?.hotelAvailability.mediumPrice || 0)}
                  onChange={(event) => parseInt(event.target.value) < 0 ? setNumberOfMediumRooms('0') : setNumberOfMediumRooms(event.target.value)}
                />
                <TextField
                  id="outlined-number"
                  label={`Large rooms ${options?.hotelAvailability.largePrice.toFixed(2)}§`}
                  type="number"
                  value={numberOfLargeRooms}
                  error={parseInt(numberOfLargeRooms) > (options?.hotelAvailability.largePrice || 0)}
                  onChange={(event) => parseInt(event.target.value) < 0 ? setNumberOfLargeRooms('0') : setNumberOfLargeRooms(event.target.value)}
                />
                <TextField
                  id="outlined-number"
                  label={`Apartment rooms ${options?.hotelAvailability.apartmentPrice.toFixed(2)}§`}
                  type="number"
                  value={numberOfApartmentRooms}
                  error={parseInt(numberOfApartmentRooms) > (options?.hotelAvailability.apartmentPrice || 0)}
                  onChange={(event) => parseInt(event.target.value) < 0 ? setNumberOfApartmentRooms('0') : setNumberOfApartmentRooms(event.target.value)}
                />
                <Typography variant="h6" sx={{width: '100%'}}>Discount</Typography>
                <TextField
                  id="outlined-number"
                  label="Discount code"
                  value={discount}
                  onChange={(event) => setDiscount(event.target.value)}
                />
              </Box>
            </Paper>
            <Box
              sx={{flexBasis: 500, flexGrow: 2, display: 'flex', flexDirection: 'column', "& .MuiPaper-root": {m: 1}}}>
              <Box sx={{display: 'flex', flexWrap: 'wrap'}}>
                <Card sx={{flexGrow: 1}}>
                  <CardMedia
                    component="img"
                    image={`https://picsum.photos/200/200?random=${Math.floor(Math.random() * 1000)}`}
                    alt="destination"
                  />
                </Card>
                <Card sx={{flexGrow: 1}}>
                  <CardMedia
                    component="img"
                    image={`https://picsum.photos/200/200?random=${Math.floor(Math.random() * 1000)}`}
                    alt="destination"
                  />
                </Card>
              </Box>
              <Box sx={{display: 'flex'}}>
                <Card>
                  <CardMedia
                    component="img"
                    image={`https://picsum.photos/200/200?random=${Math.floor(Math.random() * 1000)}`}
                    alt="destination"
                  />
                </Card>
                <Card>
                  <CardMedia
                    component="img"
                    image={`https://picsum.photos/200/200?random=${Math.floor(Math.random() * 1000)}`}
                    alt="destination"
                  />
                </Card>
                <Card>
                  <CardMedia
                    component="img"
                    image={`https://picsum.photos/200/200?random=${Math.floor(Math.random() * 1000)}`}
                    alt="destination"
                  />
                </Card>
                <Card>
                  <CardMedia
                    component="img"
                    image={`https://picsum.photos/200/200?random=${Math.floor(Math.random() * 1000)}`}
                    alt="destination"
                  />
                </Card>
                <Card>
                  <CardMedia
                    component="img"
                    image={`https://picsum.photos/200/200?random=${Math.floor(Math.random() * 1000)}`}
                    alt="destination"
                  />
                </Card>
              </Box>
              <Box sx={{display: 'flex', justifyContent: 'space-between', my: 2}}>
                {priceData?.isAvailable ? (
                  <>
                    <Typography variant="h5">Final price</Typography>
                    <Typography variant="h5">{priceData?.finalPrice}§</Typography>
                  </>
                ) : <Typography variant="h5">Offer is not available for current configuration</Typography>}
              </Box>
              <Box sx={{display: 'flex', justifyContent: 'flex-end', my: 2}}>
                <Button variant="contained" size="large">
                  Reserve
                </Button>
              </Box>
            </Box>
          </Box>
        </Box>
        <Snackbar
          open={open}
          autoHideDuration={10000}
          onClose={(event?: React.SyntheticEvent | Event, reason?: string) => {
            if (reason === 'clickaway') {
              return;
            }
            setOpen(false)
          }}>
          <Alert
            onClose={(event?: React.SyntheticEvent | Event, reason?: string) => {
              if (reason === 'clickaway') {
                return;
              }
              setOpen(false)
            }}
            severity="error"
            sx={{width: '100%'}}>
            {priceData?.validationError}
          </Alert>
        </Snackbar>
      </>
    </Layout>
  );
};

export default Offer;