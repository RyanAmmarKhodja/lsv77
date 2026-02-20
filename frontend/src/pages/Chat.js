import React, { useState, useEffect, useRef } from "react";
import { useNavigate } from "react-router-dom";
import * as signalR from "@microsoft/signalr";
import { Send, ArrowLeft, MoreVertical, Search } from "lucide-react";
import api from "../api"

const Chat = () => {
  const navigate = useNavigate();
  const [conversations, setConversations] = useState([]);
  const [selectedConversation, setSelectedConversation] = useState(null);
  const [messages, setMessages] = useState([]);
  const [messageInput, setMessageInput] = useState("");
  const [currentUser, setCurrentUser] = useState(null);
  const [connection, setConnection] = useState(null);
  const [isTyping, setIsTyping] = useState(false);
  const messagesEndRef = useRef(null);
  const typingTimeoutRef = useRef(null);

  // Fetch current user
  useEffect(() => {
    const fetchUser = async () => {
      try {
        // const token = localStorage.getItem("token");
        const response = await api.get("/users/me");
        if (response.ok) {
          const data = await response.json();
          setCurrentUser(data);
        }
      } catch (error) {
        console.error("Failed to fetch user:", error);
      }
    };

    fetchUser();
  }, []);

  // Setup SignalR connection
  useEffect(() => {
    const token = localStorage.getItem("token");
    if (!token) {
      navigate("/login");
      return;
    }

    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl("/hubs/chat", {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect()
      .build();

    setConnection(newConnection);
  }, [navigate]);

  // Start SignalR connection
  useEffect(() => {
    if (connection) {
      connection
        .start()
        .then(() => {
          console.log("Connected to chat hub");

          // Listen for incoming messages
          connection.on("ReceiveMessage", (message) => {
            if (selectedConversation && message.conversationId === selectedConversation.id) {
              setMessages((prev) => [...prev, message]);
              scrollToBottom();
            }
            // Update conversation list
            fetchConversations();
          });

          // Listen for typing indicators
          connection.on("UserTyping", ({ userId, isTyping }) => {
            if (selectedConversation) {
              setIsTyping(isTyping);
              if (isTyping) {
                setTimeout(() => setIsTyping(false), 3000);
              }
            }
          });
        })
        .catch((error) => console.error("Connection failed:", error));
    }

    return () => {
      if (connection) {
        connection.stop();
      }
    };
  }, [connection, selectedConversation]);

  // Fetch conversations
  const fetchConversations = async () => {
    try {
      const token = localStorage.getItem("token");
      const response = await api.get("/chat/conversations");
      if (response.ok) {
        const data = await response.json();
        setConversations(data);
      }
    } catch (error) {
      console.error("Failed to fetch conversations:", error);
    }
  };

  useEffect(() => {
    fetchConversations();
  }, []);

  // Fetch messages for selected conversation
  const fetchMessages = async (conversationId) => {
    try {
      const response = await api.get(`/api/chat/conversations/${conversationId}/messages?pageSize=50`);
      if (response.ok) {
        const data = await response.json();
        setMessages(data.items || []);
        scrollToBottom();
      }
    } catch (error) {
      console.error("Failed to fetch messages:", error);
    }
  };

  const handleSelectConversation = (conversation) => {
    setSelectedConversation(conversation);
    fetchMessages(conversation.id);
  };

  const handleSendMessage = async () => {
    if (!messageInput.trim() || !selectedConversation || !connection) return;

    const content = messageInput.trim();
    setMessageInput("");

    try {
      await connection.invoke("SendMessage", selectedConversation.id, content);
    } catch (error) {
      console.error("Failed to send message:", error);
      // Fallback to HTTP if SignalR fails
      const response = await api.post(`/api/chat/conversations/${selectedConversation.id}/messages`, content)
      if (response.ok) {
        const message = await response.json();
        setMessages((prev) => [...prev, message]);
      }
    }
  };

  const handleTyping = () => {
    if (connection && selectedConversation) {
      connection.invoke("TypingIndicator", selectedConversation.id, true);

      // Clear previous timeout
      if (typingTimeoutRef.current) {
        clearTimeout(typingTimeoutRef.current);
      }

      // Stop typing after 2 seconds of no input
      typingTimeoutRef.current = setTimeout(() => {
        connection.invoke("TypingIndicator", selectedConversation.id, false);
      }, 2000);
    }
  };

  const scrollToBottom = () => {
    setTimeout(() => {
      messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
    }, 100);
  };

  const formatTime = (date) => {
    const d = new Date(date);
    return d.toLocaleTimeString("fr-FR", { hour: "2-digit", minute: "2-digit" });
  };

  const getOtherParticipant = (conversation) => {
    if (!currentUser) return null;
    return conversation.participants.find((p) => p.id !== currentUser.id);
  };

  return (
    <div className="h-screen bg-gray-50 flex">
      {/* Sidebar - Conversations List */}
      <div className="w-full md:w-80 bg-white border-r border-gray-200 flex flex-col">
        {/* Header */}
        <div className="p-4 border-b border-gray-200">
          <h1 className="text-xl font-bold text-gray-900">Messages</h1>
          <div className="mt-3 relative">
            <input
              type="text"
              placeholder="Rechercher une conversation..."
              className="w-full bg-gray-100 border-none rounded-lg py-2 pl-10 pr-4 focus:ring-2 focus:ring-[#F56B2A] focus:bg-white transition-all outline-none text-sm"
            />
            <Search size={18} className="absolute left-3 top-2.5 text-gray-400" />
          </div>
        </div>

        {/* Conversations */}
        <div className="flex-1 overflow-y-auto">
          {conversations.length === 0 ? (
            <div className="p-8 text-center text-gray-500">
              <p>Aucune conversation</p>
              <p className="text-sm mt-2">Commencez à discuter avec vos camarades!</p>
            </div>
          ) : (
            conversations.map((conv) => {
              const otherUser = getOtherParticipant(conv);
              return (
                <div
                  key={conv.id}
                  onClick={() => handleSelectConversation(conv)}
                  className={`p-4 border-b border-gray-100 cursor-pointer hover:bg-gray-50 transition-colors ${
                    selectedConversation?.id === conv.id ? "bg-blue-50" : ""
                  }`}
                >
                  <div className="flex items-start gap-3">
                    <div className="w-12 h-12 bg-[#F56B2A] rounded-full flex items-center justify-center text-white font-bold flex-shrink-0">
                      {otherUser?.firstName?.[0]?.toUpperCase() || "?"}
                    </div>
                    <div className="flex-1 min-w-0">
                      <div className="flex justify-between items-baseline">
                        <h3 className="font-semibold text-gray-900 truncate">
                          {conv.name}
                        </h3>
                        {conv.lastMessageAt && (
                          <span className="text-xs text-gray-500 flex-shrink-0 ml-2">
                            {formatTime(conv.lastMessageAt)}
                          </span>
                        )}
                      </div>
                      {conv.lastMessage && (
                        <p className="text-sm text-gray-600 truncate mt-1">
                          {conv.lastMessage.sender.id === currentUser?.id ? "Vous: " : ""}
                          {conv.lastMessage.content}
                        </p>
                      )}
                    </div>
                  </div>
                </div>
              );
            })
          )}
        </div>
      </div>

      {/* Chat Area */}
      <div className="flex-1 flex flex-col">
        {selectedConversation ? (
          <>
            {/* Chat Header */}
            <div className="bg-white border-b border-gray-200 p-4 flex items-center justify-between">
              <div className="flex items-center gap-3">
                <button
                  onClick={() => setSelectedConversation(null)}
                  className="md:hidden"
                >
                  <ArrowLeft size={20} />
                </button>
                <div className="w-10 h-10 bg-[#F56B2A] rounded-full flex items-center justify-center text-white font-bold">
                  {getOtherParticipant(selectedConversation)?.firstName?.[0]?.toUpperCase() ||
                    "?"}
                </div>
                <div>
                  <h2 className="font-semibold text-gray-900">
                    {selectedConversation.name}
                  </h2>
                  {isTyping && (
                    <p className="text-xs text-gray-500">En train d'écrire...</p>
                  )}
                </div>
              </div>
              <button className="text-gray-500 hover:text-gray-700">
                <MoreVertical size={20} />
              </button>
            </div>

            {/* Messages */}
            <div className="flex-1 overflow-y-auto p-4 space-y-4 bg-gray-50">
              {messages.map((message) => {
                const isOwn = message.sender.id === currentUser?.id;
                return (
                  <div
                    key={message.id}
                    className={`flex ${isOwn ? "justify-end" : "justify-start"}`}
                  >
                    <div
                      className={`max-w-xs lg:max-w-md px-4 py-2 rounded-2xl ${
                        isOwn
                          ? "bg-[#F56B2A] text-white"
                          : "bg-white text-gray-900 border border-gray-200"
                      }`}
                    >
                      <p className="text-sm break-words">{message.content}</p>
                      <p
                        className={`text-xs mt-1 ${
                          isOwn ? "text-orange-200" : "text-gray-500"
                        }`}
                      >
                        {formatTime(message.createdAt)}
                      </p>
                    </div>
                  </div>
                );
              })}
              <div ref={messagesEndRef} />
            </div>

            {/* Message Input */}
            <div className="bg-white border-t border-gray-200 p-4">
              <div className="flex gap-2">
                <input
                  type="text"
                  value={messageInput}
                  onChange={(e) => {
                    setMessageInput(e.target.value);
                    handleTyping();
                  }}
                  onKeyPress={(e) => {
                    if (e.key === "Enter" && !e.shiftKey) {
                      e.preventDefault();
                      handleSendMessage();
                    }
                  }}
                  placeholder="Tapez votre message..."
                  className="flex-1 bg-gray-100 border-none rounded-full py-3 px-4 focus:ring-2 focus:ring-[#F56B2A] focus:bg-white transition-all outline-none text-sm"
                />
                <button
                  onClick={handleSendMessage}
                  disabled={!messageInput.trim()}
                  className="bg-[#F56B2A] hover:bg-[#E35B1D] disabled:bg-gray-300 text-white rounded-full p-3 transition-colors"
                >
                  <Send size={20} />
                </button>
              </div>
            </div>
          </>
        ) : (
          <div className="flex-1 flex items-center justify-center bg-gray-50">
            <div className="text-center text-gray-500">
              <div className="w-24 h-24 bg-gray-200 rounded-full mx-auto mb-4 flex items-center justify-center">
                <Send size={40} className="text-gray-400" />
              </div>
              <h2 className="text-xl font-semibold mb-2">Sélectionnez une conversation</h2>
              <p className="text-sm">
                Choisissez une conversation dans la liste pour commencer à discuter
              </p>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default Chat;