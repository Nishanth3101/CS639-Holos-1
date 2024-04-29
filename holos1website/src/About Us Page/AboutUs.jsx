import NavBar from "../NavBar/NavBar";
import "./aboutUs.css";
import Card from "../Card/Card";
import NishanthImage from "../assets/Nishanth.jpg";
import SakshamImage from "../assets/Saksham.jpg";
import WeibingImage from "../assets/Weibing.jpg";
import YuyangImage from "../assets/Yuyang.JPG";
import JordanImage from "../assets/Jordan.jpg"; // This line should correctly import the image

function AboutUs() {
  return (
    <>
      <NavBar />
      <header className="about-header">We're more than just a student team. We've got goals, hopes, and dreams, just like you...</header>
      <div className="team-container">
        <Card name="Nishanth Naik" description="Hi! My name is Nishanth Naik and I am a senior at UW-Madison. I enjoy gaming and I find VR games quite interesting so a VR project was very cool to work with as it was my first time using the Unity software." image={NishanthImage} />
        <Card name="Saksham Jain" description="Hi! My name's Saksham Jain and I am a Junior at UW-Madison. I enjoy learning new coding languages to deploy solutions and it was great learning Unity and C# to create an educational product in VR." image={SakshamImage} />
        <Card name="Weibing Wang" description="Description Here" image={WeibingImage} />
        <Card name="Yuyang Liu" description="I'm a senior at the University of Wisconsin-Madison, and through VR, I explore new possibilities, and it's a good idea to make learning and entertainment come alive!" image={YuyangImage} />
        <Card name="Jordan Alper" description="Hi! My name’s Jordan Alper and I’m a senior at UW-Madison! I enjoy developing video games, so a VR project was a really cool experience for me, as it uses Unity as well!" image={JordanImage} />
      </div>
    </>
  );
}

export default AboutUs;
