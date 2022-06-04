import React, {useCallback, useContext, useEffect, useState} from 'react';
import {navigate, PageProps} from "gatsby";
import Layout from "../components/Layout";
import {
  Alert,
  Box, Button,
  Card,
  CardMedia, CircularProgress,
  FormControl,
  InputLabel,
  MenuItem,
  Paper,
  Select, Snackbar,
  TextField,
  Typography
} from "@mui/material";
import {request} from "../api/request";
import {UserContext} from "../context/UserContext";
import {HubConnection, HubConnectionBuilder, LogLevel} from '@microsoft/signalr';

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
  const [reservationCode, setReservationCode] = useState('');

  const auth = useContext(UserContext);

  const [purchasedNotificationConnection, setPurchasedNotificationConnection] = useState<HubConnection | null>(null);
  const [offerUpdatedNotificationsConnection, setOfferUpdatedNotificationsConnection] = useState<HubConnection | null>(null);
  const [hotelsConnection, setHotelsConnection] = useState<HubConnection | null>(null);
  const [transportsConnection, setTransportsConnection] = useState<HubConnection | null>(null);
  const [openNotifySnackbar, setOpenNotifySnackbar] = useState(false);


  const [orderId, setOrderId] = useState('')

  useEffect(() => {
    const purchasedNotificationsHubConnection = new HubConnectionBuilder()
      .withUrl(process.env.WEB_API_URL + 'purchasedNotification')
      .configureLogging(LogLevel.Warning)
      .withAutomaticReconnect()
      .build();

    setPurchasedNotificationConnection(purchasedNotificationsHubConnection);

    const offerUpdatedNotificationsHubConnection = new HubConnectionBuilder()
      .withUrl(process.env.WEB_API_URL + 'offerUpdatedNotifications')
      .configureLogging(LogLevel.Warning)
      .withAutomaticReconnect()
      .build();

    setOfferUpdatedNotificationsConnection(offerUpdatedNotificationsHubConnection)

    const hotelHubConnection = new HubConnectionBuilder()
      .withUrl(process.env.WEB_API_URL + 'hotelsHub')
      .configureLogging(LogLevel.Warning)
      .withAutomaticReconnect()
      .build();

    setHotelsConnection(hotelHubConnection);

    const transportsHubConnection = new HubConnectionBuilder()
      .withUrl(process.env.WEB_API_URL + 'transportsHub')
      .configureLogging(LogLevel.Warning)
      .withAutomaticReconnect()
      .build();

    setTransportsConnection(transportsHubConnection);
  }, []);

  const updateOptions = useCallback(() => {
    if (!reservationCode) {
      request('POST', '/Trip/Options', {
        HotelCode: state?.trip.hotelCode,
        DepartureDate: state?.departureDate?.toISOString().split('T')[0],
        NumberOfDays: state?.numberOfDays,
        NumberOfAdults: state?.numberOfAdults
      }).then(data => {
        setOptions(data)
      })
    }
  }, [reservationCode]);

  useEffect(() => {
    updateOptions();
  }, [])

  useEffect(() => {
    if (offerUpdatedNotificationsConnection) {
      offerUpdatedNotificationsConnection.start();
      return () => {
        offerUpdatedNotificationsConnection.stop().then(() => {
          console.log("Connection stopped");
        });
      }
    }
  }, [offerUpdatedNotificationsConnection])

  useEffect(() => {
    if (offerUpdatedNotificationsConnection) {
      offerUpdatedNotificationsConnection.off("HotelUpdatedNotification");
      offerUpdatedNotificationsConnection.on("HotelUpdatedNotification", (notification) => {
        if (options?.hotelDays.some(element => {
          return notification.hotelDayIds.includes(element);
        })) {
          updateOptions();
        }
      })
      offerUpdatedNotificationsConnection.off("TransportsUpdatedNotification");
      offerUpdatedNotificationsConnection.on("TransportsUpdatedNotification", (notification) => {
        if (options?.returnTransportOptions.concat(options?.transportOptions).some(element => {
          return notification.transportIds.includes(element.id);
        })) {
          updateOptions();
        }
      })
    }
    if (purchasedNotificationConnection && options) {
      purchasedNotificationConnection.off("SendNotification")
      purchasedNotificationConnection.on("SendNotification", (notification: { orderId: string, purchasedHotelDays: string[] }) => {
        if (notification.orderId !== orderId && options?.hotelDays.some(element => {
          return notification.purchasedHotelDays.includes(element);
        })) {
          setOpenNotifySnackbar(true);
        }
      })
    }
  }, [purchasedNotificationConnection, offerUpdatedNotificationsConnection, options, reservationCode])

  useEffect(() => {
    if (orderId && purchasedNotificationConnection) {
      purchasedNotificationConnection.off("SendNotification");
      purchasedNotificationConnection.on("SendNotification", (notification: { orderId: string, purchasedHotelDays: string[] }) => {
        if (notification.orderId !== orderId && options?.hotelDays.some(element => {
          return notification.purchasedHotelDays.includes(element);
        })) {
          setOpenNotifySnackbar(true);
        }
      })
    }
  }, [orderId, purchasedNotificationConnection])

  useEffect(() => {
    if (purchasedNotificationConnection) {
      purchasedNotificationConnection.start()
        .catch(e => console.log('Connection failed: ', e));
      return () => {
        purchasedNotificationConnection.stop().then(() => {
          console.log("Connection stopped");
        });
      }
    }
  }, [purchasedNotificationConnection])

  const [hotelStatistics, setHotelStatistics] = useState<{
    roomsStudio: number,
    roomsSmall: number,
    roomsMedium: number,
    roomsLarge: number,
    roomsApartment: number
  }>();

  useEffect(() => {
    if (hotelsConnection) {
      hotelsConnection.start()
        .then(() => {
          hotelsConnection.on('HotelResponse', response => {
            setHotelStatistics(response)
          })
          hotelsConnection.invoke('GetForHotel', {Destination: state?.airportCode, HotelCode: state?.trip?.hotelCode})
          hotelsConnection.on("HotelCountUpdate", (response) => {
            setHotelStatistics(response)
          })
        }).catch(e => console.log('Connection failed: ', e));
      return () => {
        hotelsConnection.stop().then(() => {
          console.log("Connection stopped");
        });
      }
    }
  }, [hotelsConnection])

  const [transportsStatistics, setTransportStatistics] = useState<{
    transports: {
      count: 1
      destinationAirportCode: string | null
    }[],
    returnTransports: {
      count: 1
      destinationAirportCode: string | null
    }[]
  }>({transports: [], returnTransports: []});

  useEffect(() => {
    if (transportsConnection) {
      transportsConnection.start()
        .then(() => {
          transportsConnection.on('TransportCountsUpdate', response => {
            setTransportStatistics({
              transports: response.transports,
              returnTransports: response.returnTransports
            })
          })
          transportsConnection.invoke('GetForDestination', {Destination: state?.airportCode})
        }).catch(e => console.log('Connection failed: ', e));
      return () => {
        transportsConnection.stop().then(() => {
          console.log("Connection stopped");
        });
      }
    }
  }, [transportsConnection])

  const [numberOfAdults, setNumberOfAdults] = useState<string>(state?.numberOfAdults)
  const [departure, setDeparture] = useState(state && state.departure !== 'any' ? state.departure : 'individual' ?? 'individual')
  const [arrival, setArrival] = useState(state && state.departure !== 'any' ? state.departure : 'individual' ?? 'individual')
  const [numberOfChildrenUpTo18, setNumberOfChildrenUpTo18] = useState<string>(state?.numberOfChildrenUpTo18)
  const [numberOfChildrenUpTo10, setNumberOfChildrenUpTo10] = useState<string>(state?.numberOfChildrenUpTo10)
  const [numberOfChildrenUpTo3, setNumberOfChildrenUpTo3] = useState<string>(state?.numberOfChildrenUpTo3)
  const [mealOption, setMealOption] = useState(options?.hotelAvailability.allInclusive ? '2' : '1');
  const [discount, setDiscount] = useState('');
  const [numberOfStudioRooms, setNumberOfStudioRooms] = useState<string>('')
  const [numberOfSmallRooms, setNumberOfSmallRooms] = useState<string>('')
  const [numberOfMediumRooms, setNumberOfMediumRooms] = useState<string>('')
  const [numberOfLargeRooms, setNumberOfLargeRooms] = useState<string>('')
  const [numberOfApartmentRooms, setNumberOfApartmentRooms] = useState<string>('')

  const [priceData, setPriceData] = useState<{ validationError?: string | null, isAvailable: boolean, finalPrice: number } | null>(null);

  const [loading, setLoading] = useState(false);

  const [paymentStatus, setPaymentStatus] = useState('');
  const [paymentLoading, setPaymentLoading] = useState(false);
  const [orderStatus, setOrderStatus] = useState('');
  const [orderLoading, setOrderLoading] = useState(false);

  const [paymentTimeout, setPaymentTimeout] = useState('');

  useEffect(() => {
    setLoading(true);
    request('POST', '/Trip', {
      Flights: [
        departure !== 'individual' ? options?.transportOptions.find((transport) => transport.destinationAirportCode === departure)?.id : null,
        arrival !== 'individual' ? options?.returnTransportOptions.find((transport) => transport.destinationAirportCode === arrival)?.id : null,
      ].filter(element => element != null),
      HotelDays: options == null ? [] : options.hotelDays,
      NumberOfAdults: numberOfAdults,
      NumberOfChildrenUpTo18: numberOfChildrenUpTo18 ? numberOfChildrenUpTo18 : 0,
      NumberOfChildrenUpTo10: numberOfChildrenUpTo10 ? numberOfChildrenUpTo10 : 0,
      NumberOfChildrenUpTo3: numberOfChildrenUpTo3 ? numberOfChildrenUpTo3 : 0,
      NumberOfStudios: numberOfStudioRooms ? numberOfStudioRooms : 0,
      NumberOfSmallRooms: numberOfSmallRooms ? numberOfSmallRooms : 0,
      NumberOfMediumRooms: numberOfMediumRooms ? numberOfMediumRooms : 0,
      NumberOfLargeRooms: numberOfLargeRooms ? numberOfLargeRooms : 0,
      NumberOfApartments: numberOfApartmentRooms ? numberOfApartmentRooms : 0,
      MealOption: mealOption,
      DiscountCode: discount,
    }).then(data => {
      if (data?.validationError) {
        setOpen(true);
      } else {
        setOpen(false)
      }
      setPriceData(data);
      setLoading(false)
    })
  }, [arrival, departure, mealOption, numberOfAdults, numberOfApartmentRooms, numberOfChildrenUpTo10, numberOfChildrenUpTo18, numberOfChildrenUpTo3, numberOfLargeRooms, numberOfMediumRooms, numberOfSmallRooms, numberOfStudioRooms, options?.hotelDays, options?.returnTransportOptions, options?.transportOptions, discount])

  if (state == null) {
    return (
      <Layout>
        <>
          <Typography variant="h2" sx={{m: 2}}>
            No selected offer
          </Typography>
          <Button variant="contained" size="large" sx={{m: 2}} onClick={() => navigate("/trips")}>
            Return to trips
          </Button>
        </>
      </Layout>
    )
  }

  const checkOrder = (reservationId: string) => {
    setOrderId(reservationId);
    setOrderLoading(true);
    request('GET', `/Order/${reservationId}`, '', auth.user)
      .then((data) => {
        if (data.payment?.status !== 'New' && data.order?.state !== 'Rejected') {
          setTimeout(() => checkOrder(reservationId), 500);
        } else {
          setPaymentStatus(data.payment?.status);
          setOrderStatus(data.order?.state);
          setOrderLoading(false)
        }
      }).catch(error => console.log(error));
  }

  const checkPayment = () => {
    request('GET', `/Order/${reservationCode}`, '', auth.user)
      .then((data) => {
        if (data.payment?.status !== 'Accepted' && data.payment?.status !== 'Rejected' && data.payment?.status !== 'Timeout') {
          setTimeout(() => checkPayment(), 500);
        } else if (data.payment?.status === 'Timeout') {
          setOrderStatus('')
          setPaymentStatus('')
          setPaymentLoading(false)
          setPaymentTimeout(data.payment?.status)
        } else if (data.payment?.status === 'Rejected') {
          setPaymentLoading(false)
          setPaymentStatus(data.payment?.status)
        } else {
          setPaymentStatus(data.payment.status)
          setPaymentLoading(false)
        }
      }).catch(error => console.log(error));
  }

  const pay = () => {
    setPaymentLoading(true);
    request('POST', `/Order/Pay/${reservationCode}`, {}, auth.user)
      .then((data) => {
        if (!data.payment?.status) {
          setTimeout(() => checkPayment(), 500);
        }
      }).catch(error => console.log(error));
  }

  const reserve = () => {
    setPaymentTimeout('');
    request('POST', '/Order/Submit', {
      Order: {
        TransportId: departure !== 'individual' ? options?.transportOptions.find((transport) => transport.destinationAirportCode === departure)?.id : null,
        ReturnTransportId: arrival !== 'individual' ? options?.returnTransportOptions.find((transport) => transport.destinationAirportCode === arrival)?.id : null,
        HotelDays: options == null ? [] : options.hotelDays,
        NumberOfAdults: numberOfAdults,
        NumberOfChildrenUpTo18: numberOfChildrenUpTo18 ? numberOfChildrenUpTo18 : 0,
        NumberOfChildrenUpTo10: numberOfChildrenUpTo10 ? numberOfChildrenUpTo10 : 0,
        NumberOfChildrenUpTo3: numberOfChildrenUpTo3 ? numberOfChildrenUpTo3 : 0,
        roomsStudio: numberOfStudioRooms ? numberOfStudioRooms : 0,
        roomsSmall: numberOfSmallRooms ? numberOfSmallRooms : 0,
        roomsMedium: numberOfMediumRooms ? numberOfMediumRooms : 0,
        roomsLarge: numberOfLargeRooms ? numberOfLargeRooms : 0,
        roomsApartment: numberOfApartmentRooms ? numberOfApartmentRooms : 0,
        MealOption: mealOption,
        DiscountCode: discount,
        HotelCode: state.trip.hotelCode,
        userName: auth.user.username,
      }
    }, auth.user).then((data) => {
      setReservationCode(data);
      checkOrder(data);
    }).catch(error => console.log(error));
  }

  return (
    <Layout>
      <>
        <Box sx={{py: 2}}>
          <Typography variant="h5" sx={{mb: 2}}>{state?.trip.hotelName}</Typography>
          <Box sx={{display: 'flex'}}>
            <Paper sx={{flexBasis: 500, flexGrow: 1, p: 2, mr: 2}}>
              <Typography variant="h5">Options</Typography>
              <Typography color="text.secondary">All prices are approximate</Typography>
              <Box
                sx={{display: 'flex', flexWrap: 'wrap', '& .MuiFormControl-root': {m: 1, minWidth: 200, flexGrow: 1}}}>
                <TextField
                  id="outlined-number"
                  label="Departure date"
                  value={state?.departureDate?.toISOString().split('T')[0]}
                  variant="standard"
                  disabled
                />
                <TextField
                  id="outlined-number"
                  label="Number of days"
                  value={state?.numberOfDays}
                  variant="standard"
                  disabled
                />
                <FormControl fullWidth disabled={(!!orderStatus || orderLoading) && orderStatus !== 'Rejected'}>
                  <InputLabel id="departure" required>Departure Airport</InputLabel>
                  <Select
                    id="departure"
                    label="Departure Airport"
                    value={departure}
                    onChange={(event) => setDeparture(event.target.value)}
                  >
                    <MenuItem value='individual' key='individual'>
                      Individual transport
                      <span style={{marginLeft: 'auto'}}>
                        {(() => {
                          const transportStatistic = transportsStatistics.transports.find(statistic => statistic.destinationAirportCode === null);
                          if (transportStatistic) {
                            return ` ${transportStatistic.count} just booked!`
                          }
                          return ''
                        })()}
                      </span>
                    </MenuItem>
                    {options?.transportOptions.map(transport => (
                      <MenuItem value={transport.destinationAirportCode} key={transport.destinationAirportCode}>
                        {transport.destinationAirportName} from {transport.ticketPrice}§/place
                        <span style={{marginLeft: 'auto'}}>
                        {(() => {
                          const transportStatistic = transportsStatistics.transports
                            .find(statistic => statistic.destinationAirportCode === transport.destinationAirportCode);
                          if (transportStatistic) {
                            return ` ${transportStatistic.count} just booked!`
                          }
                          return ''
                        })()}
                      </span>
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
                <FormControl fullWidth disabled={(!!orderStatus || orderLoading) && orderStatus !== 'Rejected'}>
                  <InputLabel id="arrival" required>Arrival Airport</InputLabel>
                  <Select
                    id="arrival"
                    label="Arrival Airport"
                    value={arrival}
                    onChange={(event) => setArrival(event.target.value)}
                  >
                    <MenuItem value='individual' key='individual'>
                      Individual transport
                      <span style={{marginLeft: 'auto'}}>
                        {(() => {
                          const transportStatistic = transportsStatistics.returnTransports.find(statistic => statistic.destinationAirportCode === null);
                          if (transportStatistic) {
                            return ` ${transportStatistic.count} just booked!`
                          }
                          return ''
                        })()}
                      </span>
                    </MenuItem>
                    {options?.returnTransportOptions.map(transport => (
                      <MenuItem value={transport.destinationAirportCode} key={transport.destinationAirportCode}>
                        {transport.destinationAirportName} from {transport.ticketPrice}§/place
                        <span style={{marginLeft: 'auto'}}>
                        {(() => {
                          const transportStatistic = transportsStatistics.returnTransports
                            .find(statistic => statistic.destinationAirportCode === transport.destinationAirportCode);
                          if (transportStatistic) {
                            return ` ${transportStatistic.count} just booked!`
                          }
                          return ''
                        })()}
                      </span>
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
                <TextField
                  required
                  label="Number of adults"
                  type="number"
                  value={numberOfAdults}
                  disabled={(!!orderStatus || orderLoading) && orderStatus !== 'Rejected'}
                  onChange={(event) => parseInt(event.target.value) < 1 ? setNumberOfAdults('1') : setNumberOfAdults(event.target.value)}
                />
                <TextField
                  label="Children up to 18"
                  type="number"
                  value={numberOfChildrenUpTo18}
                  disabled={(!!orderStatus || orderLoading) && orderStatus !== 'Rejected'}
                  onChange={(event) => parseInt(event.target.value) < 0 ? setNumberOfChildrenUpTo18('0') : setNumberOfChildrenUpTo18(event.target.value)}
                />
                <TextField
                  label="Children up to 10"
                  type="number"
                  value={numberOfChildrenUpTo10}
                  disabled={(!!orderStatus || orderLoading) && orderStatus !== 'Rejected'}
                  onChange={(event) => parseInt(event.target.value) < 0 ? setNumberOfChildrenUpTo10('0') : setNumberOfChildrenUpTo10(event.target.value)}
                />
                <TextField
                  label="Children up to 3"
                  type="number"
                  value={numberOfChildrenUpTo3}
                  disabled={(!!orderStatus || orderLoading) && orderStatus !== 'Rejected'}
                  onChange={(event) => parseInt(event.target.value) < 0 ? setNumberOfChildrenUpTo3('0') : setNumberOfChildrenUpTo3(event.target.value)}
                />
                <FormControl fullWidth disabled={(!!orderStatus || orderLoading) && orderStatus !== 'Rejected'}>
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
                  id="outlined-number1"
                  label={`Studio rooms ${options?.hotelAvailability.studioPrice.toFixed(2)}§`}
                  type="number"
                  value={numberOfStudioRooms}
                  error={parseInt(numberOfStudioRooms) > (options?.hotelAvailability.roomsStudio || 0)}
                  helperText={hotelStatistics?.roomsStudio ? `${hotelStatistics?.roomsStudio} ${hotelStatistics?.roomsStudio > 1 ? 'users' : 'user'} have just chosen this room!` : undefined}
                  disabled={(!!orderStatus || orderLoading) && orderStatus !== 'Rejected'}
                  onChange={(event) => parseInt(event.target.value) < 0 ? setNumberOfStudioRooms('0') : setNumberOfStudioRooms(event.target.value)}
                />
                <TextField
                  id="outlined-number2"
                  label={`Small rooms ${options?.hotelAvailability.smallPrice.toFixed(2)}§`}
                  type="number"
                  value={numberOfSmallRooms}
                  error={parseInt(numberOfSmallRooms) > (options?.hotelAvailability.roomsSmall || 0)}
                  helperText={hotelStatistics?.roomsSmall ? `${hotelStatistics?.roomsSmall} ${hotelStatistics?.roomsSmall > 1 ? 'users' : 'user'} have just chosen this room!` : undefined}
                  disabled={(!!orderStatus || orderLoading) && orderStatus !== 'Rejected'}
                  onChange={(event) => parseInt(event.target.value) < 0 ? setNumberOfSmallRooms('0') : setNumberOfSmallRooms(event.target.value)}
                />
                <TextField
                  id="outlined-number3"
                  label={`Medium rooms ${options?.hotelAvailability.mediumPrice.toFixed(2)}§`}
                  type="number"
                  value={numberOfMediumRooms}
                  error={parseInt(numberOfMediumRooms) > (options?.hotelAvailability.roomsMedium || 0)}
                  helperText={hotelStatistics?.roomsMedium ? `${hotelStatistics?.roomsMedium} ${hotelStatistics?.roomsMedium > 1 ? 'users' : 'user'} have just chosen this room!` : undefined}
                  disabled={(!!orderStatus || orderLoading) && orderStatus !== 'Rejected'}
                  onChange={(event) => parseInt(event.target.value) < 0 ? setNumberOfMediumRooms('0') : setNumberOfMediumRooms(event.target.value)}
                />
                <TextField
                  id="outlined-number4"
                  label={`Large rooms ${options?.hotelAvailability.largePrice.toFixed(2)}§`}
                  type="number"
                  value={numberOfLargeRooms}
                  error={parseInt(numberOfLargeRooms) > (options?.hotelAvailability.roomsLarge || 0)}
                  helperText={hotelStatistics?.roomsLarge ? `${hotelStatistics?.roomsLarge} ${hotelStatistics?.roomsLarge > 1 ? 'users' : 'user'} have just chosen this room!` : undefined}
                  disabled={(!!orderStatus || orderLoading) && orderStatus !== 'Rejected'}
                  onChange={(event) => parseInt(event.target.value) < 0 ? setNumberOfLargeRooms('0') : setNumberOfLargeRooms(event.target.value)}
                />
                <TextField
                  id="outlined-number5"
                  label={`Apartment rooms ${options?.hotelAvailability.apartmentPrice.toFixed(2)}§`}
                  type="number"
                  value={numberOfApartmentRooms}
                  error={parseInt(numberOfApartmentRooms) > (options?.hotelAvailability.roomsApartment || 0)}
                  helperText={hotelStatistics?.roomsApartment ? `${hotelStatistics?.roomsApartment} ${hotelStatistics?.roomsApartment > 1 ? 'users' : 'user'} have just chosen this room!` : undefined}
                  disabled={(!!orderStatus || orderLoading) && orderStatus !== 'Rejected'}
                  onChange={(event) => parseInt(event.target.value) < 0 ? setNumberOfApartmentRooms('0') : setNumberOfApartmentRooms(event.target.value)}
                />
                <Typography variant="h6" sx={{width: '100%'}}>Discount</Typography>
                <TextField
                  id="outlined-number6"
                  label="Discount code"
                  value={discount}
                  disabled={(!!orderStatus || orderLoading) && orderStatus !== 'Rejected'}
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
                    image={`https://picsum.photos/200/200?random=34611`}
                    alt="destination"
                  />
                </Card>
                <Card sx={{flexGrow: 1}}>
                  <CardMedia
                    component="img"
                    image={`https://picsum.photos/200/200?random=22135`}
                    alt="destination"
                  />
                </Card>
              </Box>
              <Box sx={{display: 'flex'}}>
                <Card>
                  <CardMedia
                    component="img"
                    image={`https://picsum.photos/200/200?random=38976`}
                    alt="destination"
                  />
                </Card>
                <Card>
                  <CardMedia
                    component="img"
                    image={`https://picsum.photos/200/200?random=253`}
                    alt="destination"
                  />
                </Card>
                <Card>
                  <CardMedia
                    component="img"
                    image={`https://picsum.photos/200/200?random=654`}
                    alt="destination"
                  />
                </Card>
                <Card>
                  <CardMedia
                    component="img"
                    image={`https://picsum.photos/200/200?random=7452`}
                    alt="destination"
                  />
                </Card>
                <Card>
                  <CardMedia
                    component="img"
                    image={`https://picsum.photos/200/200?random=31461435`}
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
                {(paymentStatus === 'New' || paymentStatus === 'Rejected') &&
                  <Button
                    variant="contained"
                    size="large"
                    disabled={state === null || loading || !priceData?.isAvailable || !auth.user.username || paymentLoading}
                    onClick={pay}
                  >
                    Pay
                  </Button>}
                {!paymentStatus &&
                  <Button
                    variant="contained"
                    size="large"
                    disabled={state === null || loading || !priceData?.isAvailable || !auth.user.username || orderLoading}
                    onClick={reserve}
                  >
                    Reserve
                  </Button>}
              </Box>
              {orderStatus === 'Rejected' && <Typography>
                Order has been rejected!
              </Typography>}
              {paymentStatus === 'Rejected' && <Typography>
                Payment has been rejected!
              </Typography>}
              {paymentStatus === 'Accepted' && <Typography>
                Payment has been accepted. Order confirmed.
              </Typography>}
              {paymentTimeout === 'Timeout' && <Typography>
                Payment expired. Order has been cancelled.
              </Typography>}
              <Box sx={{display: 'flex', justifyContent: 'flex-end', my: 2}}>
                {orderLoading && <Box sx={{display: 'flex', flexDirection: 'column', alignItems: 'flex-end'}}>
                  <Typography>Waiting for reservation...</Typography>
                  <CircularProgress/>
                </Box>}
              </Box>
              <Box sx={{display: 'flex', justifyContent: 'flex-end', my: 2}}>
                {paymentLoading && <Box sx={{display: 'flex', flexDirection: 'column', alignItems: 'flex-end'}}>
                  <Typography>Waiting for payment...</Typography>
                  <CircularProgress/>
                </Box>}
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
        <Snackbar
          open={openNotifySnackbar}
          autoHideDuration={10000}
          onClose={(event?: React.SyntheticEvent | Event, reason?: string) => {
            if (reason === 'clickaway') {
              return;
            }
            setOpenNotifySnackbar(false)
          }}>
          <Alert
            onClose={(event?: React.SyntheticEvent | Event, reason?: string) => {
              if (reason === 'clickaway') {
                return;
              }
              setOpenNotifySnackbar(false)
            }}
            severity="info"
            sx={{width: '100%'}}>
            HURRY UP! Someone has just bought a similar offer.
          </Alert>
        </Snackbar>
      </>
    </Layout>
  );
};

export default Offer;