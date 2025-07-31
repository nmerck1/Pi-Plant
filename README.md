# 🌱 Pi-Plant

This is a **full-stack plant monitoring system** that runs on my Raspberry Pi home server. 

**Tech Stack:**
- 🔧 **Backend**: Go (as a backend API)
- ⚛️ **Frontend**: React (because who doesn't love components?)
- 🥧 **Hosting**: Raspberry Pi 4 (my little green server for my green plants!)
- 📊 **Database**: SQLite (lightweight and Pi-friendly)
- 🌐 **Deployment**: Self-hosted on my home network

## 🌟 Features 

- 📈 **Real-time monitoring** of soil moisture, temperature, sunlight, and humidity
- 📱 **Responsive dashboard** - check on your plants from anywhere in the house
- 📊 **Historical data tracking** - see how your plants have been doing over time
- 🚨 **Smart alerts** - get notified when your plants need attention (coming soon!)
- 🎨 **Clean, modern UI** - because data should look as good as it functions

### Backend Magic ✨
The Go API handles all the heavy lifting:
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

This entire application runs 24/7 on my Raspberry Pi 4 sitting quietly in my office on my bookshelf.

**Why Raspberry Pi?**
- 💰 Cost-effective
- 🌱 Energy efficient
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

## 🌱 What's Next?

- [ ] Mobile app for on-the-go plant checking
- [ ] Email/SMS notifications for thirsty plants
- [ ] Plant care recommendations based on data trends
- [ ] Support for multiple plant types and custom thresholds
- [ ] Maybe a plant emoji status system? 🌵➡️🌿➡️🌳

## 🤝 Contributing

Found a bug? Have an idea for making plants even happier? Feel free to open an issue or submit a PR! This is a learning project, so all skill levels are welcome.

## 📸 Screenshots

*Coming soon!*

## 🎯 Why I Built This

Well, my wife loves plants and I love my wife. Nuff said.  

---

**Made with ❤️ and lots of ☕ on a Raspberry Pi**

*P.S. - My plants are still alive, so I consider this project a success!* 🎉
