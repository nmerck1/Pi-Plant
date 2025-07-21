# 🌱 Pi-Plant

> *Keeping my green friends happy, one sensor at a time!* 🌿

## 🚀 What's This All About?

Welcome to my little corner of the internet where technology meets botany! This is a **full-stack plant monitoring system** that runs on my trusty Raspberry Pi home server. Because why buy expensive plant monitors when you can build your own and learn some cool tech along the way? 

**Tech Stack:**
- 🔧 **Backend**: C# with ASP.NET Core Web API
- ⚛️ **Frontend**: React (because who doesn't love components?)
- 🥧 **Hosting**: Raspberry Pi 4 (my little green server for my green plants!)
- 📊 **Database**: SQLite (lightweight and Pi-friendly)
- 🌐 **Deployment**: Self-hosted on my home network

## 🌟 Features That Make Plants Happy

- 📈 **Real-time monitoring** of soil moisture, temperature, and humidity
- 📱 **Responsive dashboard** - check on your plants from anywhere in the house
- 📊 **Historical data tracking** - see how your plants have been doing over time
- 🚨 **Smart alerts** - get notified when your plants need attention (coming soon!)
- 🎨 **Clean, modern UI** - because data should look as good as it functions

## 🛠️ The Tech Journey

This project started as "I wonder if I can make my Raspberry Pi do more than just store files..." and turned into a full-blown plant parenthood assistant! It's built with:

### Backend Magic ✨
The C# API handles all the heavy lifting:
- Collecting sensor data from GPIO pins
- Storing everything in a lightweight SQLite database
- Serving up clean REST endpoints for the frontend
- Running smoothly on ARM64 architecture

### Frontend Fun 🎨
The React app brings the data to life:
- Real-time updates without page refreshes
- Interactive charts and graphs
- Mobile-friendly responsive design
- Satisfying animations (because why not?)

## 🏠 Home Server Hosting

This entire application runs 24/7 on my Raspberry Pi 4 sitting quietly in my living room. No cloud costs, no external dependencies - just pure self-hosted goodness! The Pi serves the React app, handles API requests, and talks to the sensors all while sipping power like a responsible citizen.

**Why Raspberry Pi?**
- 💰 Cost-effective
- 🌱 Energy efficient (perfect for a plant monitor!)
- 🔧 Fun to tinker with
- 📚 Great learning experience

## 🚀 Getting Started

```bash
# Clone this green goodness
git clone https://github.com/yourusername/Pi-Plant.git
cd Pi-Plant

# Restore packages
dotnet restore

# Run the magic
dotnet run
```

Navigate to `http://localhost:5000` and watch your plant data come alive!

## 🌱 What's Growing Next?

- [ ] Mobile app for on-the-go plant checking
- [ ] Email/SMS notifications for thirsty plants
- [ ] Plant care recommendations based on data trends
- [ ] Support for multiple plant types and custom thresholds
- [ ] Maybe a cute plant emoji status system? 🌵➡️🌿➡️🌳

## 🤝 Contributing

Found a bug? Have an idea for making plants even happier? Feel free to open an issue or submit a PR! This is a learning project, so all skill levels are welcome.

## 📸 Screenshots

*Coming soon! Currently too busy making sure my plants don't die while I code...*

## 🎯 Why I Built This

As a developer who can barely keep a cactus alive, I figured technology might help bridge that gap. Plus, combining my love for coding with the challenge of not killing innocent plants seemed like a win-win situation!

---

**Made with ❤️ and lots of ☕ on a Raspberry Pi**

*P.S. - My plants are still alive, so I consider this project a success!* 🎉
